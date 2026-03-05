using System;
using System.Collections.Generic;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Gameplay.Targeting;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public abstract class BaliseHellaAbilityStrategy<TData> : IAbilityStrategy<BaliseHellaData, TData>
        where TData : IBaliseHellaAbilityStrategyData, IAbilityStrategyData
    {
        protected readonly struct DetectedPlayer : IEquatable<DetectedPlayer>
        {
            public readonly ITargetable targetable;
            public readonly IHealth health;
            public readonly IHealable healable;
        
            public DetectedPlayer(GameObject go)
            {
                targetable = go.GetComponent<ITargetable>();
                health = go.GetComponent<IHealth>();
                healable = go.GetComponent<IHealable>();
            }

            public bool Equals(DetectedPlayer other)
            {
                return Equals(targetable, other.targetable) && Equals(health, other.health);
            }

            public override bool Equals(object obj)
            {
                return obj is DetectedPlayer other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(targetable, health);
            }
        }
        
        protected BaliseHella CurrentBalise { get; private set; }
        protected TData Data { get; private set; }
        protected ICaster Caster { get; private set; }

        protected Dictionary<GameObject, DetectedPlayer> players;

        private NetworkSpawnManager spawnManager;
        private LocalGamePlayer localPlayer;

        private float currentTime;

        public void Initialize(IAbility<BaliseHellaData> ability, ICaster caster, TData data)
        {
            Caster = caster;
            Data = data;
            
            players = new Dictionary<GameObject, DetectedPlayer>(4);
            
            spawnManager = NetworkManager.Singleton.SpawnManager;
            localPlayer = GamePlayerManager.Instance.GetLocalPlayer();
        }

        public virtual void Begin(IAbility<BaliseHellaData> ability)
        {
            currentTime = 0;
            
            NetworkObject networkObject =
                spawnManager.InstantiateAndSpawn(ability.DataT.BalisePrefab, 
                    localPlayer.ClientID, 
                    true, 
                    false, 
                    false, 
                    Caster.CastAnchor.position + Caster.Forward, Quaternion.identity);

            if (networkObject.TryGetComponent(out BaliseHella balise))
            {
                CurrentBalise = balise;
                
                CurrentBalise.Initialize(ability.DataT, Caster.Forward, Data.Radius, Data.Duration);
            
                CurrentBalise.DetectionArea.OnEnter += OnEnter;
                CurrentBalise.DetectionArea.OnExit += OnExit;
            }
        }

        public virtual void Tick(IAbility<BaliseHellaData> ability, float deltaTime)
        {
            currentTime += deltaTime;
            if (currentTime >= Data.Duration)
            {
                ability.End();
            }
        }

        public virtual void End(IAbility<BaliseHellaData> ability)
        {
            CurrentBalise.DetectionArea.OnEnter -= OnEnter;
            CurrentBalise.DetectionArea.OnExit -= OnExit;
            
            CurrentBalise.Stop();
            CurrentBalise = null;
        }

        public void Dispose(IAbility<BaliseHellaData> ability)
        {
        }

        protected virtual void OnEnter(Collider col, object obj)
        {
            players.Add(col.gameObject, new DetectedPlayer(col.gameObject));
        }

        protected virtual void OnExit(Collider col, object obj)
        {
            players.Remove(col.gameObject);
        }
    }
    
    [CreateStrategyFor(typeof(BaliseHellaStrategyData))]
    public class BaliseHellaAbilityStrategy : BaliseHellaAbilityStrategy<BaliseHellaStrategyData>
    {
        protected override void OnEnter(Collider col, object obj)
        {
            base.OnEnter(col, obj);
            players[col.gameObject].health.SetMinHealth(Data.MinHealth);
        }

        protected override void OnExit(Collider col, object obj)
        {
            players[col.gameObject].health.SetMinHealth(0);
            base.OnExit(col, obj);
        }
    }
    
    [CreateStrategyFor(typeof(BaliseSecondChanceStrategyData))]
    public class BaliseSecondChanceAbilityStrategy : BaliseHellaAbilityStrategy<BaliseSecondChanceStrategyData>
    {
        protected override void OnEnter(Collider col, object obj)
        {
            base.OnEnter(col, obj);
            players[col.gameObject].health.SetMinHealth(Data.MinHealth);
        }

        protected override void OnExit(Collider col, object obj)
        {
            players[col.gameObject].health.SetMinHealth(0);
            base.OnExit(col, obj);
        }
        
        public override void Tick(IAbility<BaliseHellaData> ability, float deltaTime)
        {
            foreach ((GameObject key, DetectedPlayer value) in players)
            {
                value.healable.Heal(Data.HealingPerSecond * deltaTime);
            }
        }
    }
    
    [CreateStrategyFor(typeof(BaliseInvisibilityStrategyData))]
    public class BaliseInvisibilityAbilityStrategy : BaliseHellaAbilityStrategy<BaliseInvisibilityStrategyData>
    {
        protected override void OnEnter(Collider col, object obj)
        {
            base.OnEnter(col, obj);
            players[col.gameObject].targetable.SetTargetable(false);
        }

        protected override void OnExit(Collider col, object obj)
        {
            players[col.gameObject].targetable.SetTargetable(true);
            base.OnExit(col, obj);
        }
    }
}