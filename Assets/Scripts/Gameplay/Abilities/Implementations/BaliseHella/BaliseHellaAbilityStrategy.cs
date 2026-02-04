using System;
using System.Collections.Generic;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Gameplay.Targeting;
using UnityEngine;
using Object = UnityEngine.Object;

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
        
        protected BaliseHella Balise { get; private set; }
        protected TData Data { get; private set; }
        protected ICaster Caster { get; private set; }

        protected Dictionary<GameObject, DetectedPlayer> players;

        public void Initialize(IAbility<BaliseHellaData> ability, ICaster caster, TData data)
        {
            Caster = caster;
            Data = data;
            
            players = new Dictionary<GameObject, DetectedPlayer>(4);
        }
        
        public virtual void Begin(IAbility<BaliseHellaData> ability)
        {
            Balise = Object.Instantiate(ability.DataT.BalisePrefab, Caster.transform.position + Caster.Forward, Quaternion.identity);
            Balise.Initialize(ability.DataT, Caster.Forward, Data.Radius);
            
            Balise.DetectionArea.OnEnter += OnEnter;
            Balise.DetectionArea.OnExit += OnExit;
        }

        public virtual void Tick(IAbility<BaliseHellaData> ability, float deltaTime)
        {
        }

        public virtual void End(IAbility<BaliseHellaData> ability)
        {
            Balise.DetectionArea.OnEnter -= OnEnter;
            Balise.DetectionArea.OnExit -= OnExit;
            
            Balise.Stop();
            Balise = null;
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