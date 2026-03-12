using System.Collections.Generic;
using Helteix.Tools;
using OverBang.ExoWorld.Core.Components;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Gameplay.Targeting;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class GazDispenser : NetworkBehaviour
    {
        [SerializeField] private QuestThreeData data;
        [SerializeField] private DetectionArea detectionArea;
        
        private List<IDamageable> targets;
        private DynamicBuffer<IDamageable> buffer;
        
        private float timer;
        private const float SECOND = 1f;

        private bool isActive;

        private void Awake()
        {
            if (!IsOwner)
                this.enabled = false;

            SetActiveState(true);
            
            targets = new List<IDamageable>(4);
            buffer = new DynamicBuffer<IDamageable>(4);
            
            detectionArea.SetAllowedTags("Player", "LocalPlayer");
            detectionArea.SetRequireInterface<IDamageable>();
        }

        private void OnEnable()
        {
            detectionArea.OnEnter += OnTargetEnter;
            detectionArea.OnExit += OnTargetExit;
        }

        private void OnDisable()
        {
            detectionArea.OnEnter -= OnTargetEnter;
            detectionArea.OnExit -= OnTargetExit;
        }

        private void Update()
        {
            if (!isActive) 
                return;
            
            timer += Time.deltaTime;
            if (timer >= SECOND)
            {
                timer = 0f;
                ApplyGazDamage();
            }
        }

        public void SetActiveState(bool state)
        {
            if (!IsOwner) 
                return;
            
            isActive = state;
        }

        private void ApplyGazDamage()
        {
            buffer.CopyFrom(targets);
            for (int i = 0; i < buffer.Length; i++)
            {
                IDamageable target = buffer[i];
                
                Debug.Log($"Applying {data.GazTickDamage} Damage to {target.GetType().Name}");

                RuntimeDamageData damageData = new RuntimeDamageData()
                {
                    finalDamage = data.GazTickDamage.baseDamage,
                    weakSpotMultiplier = data.GazTickDamage.weakSpotMultiplier,
                };
                
                target.TakeDamage(damageData);
            }
        }

        private void OnTargetEnter(Collider col, object target)
        {
            if (target is IDamageable damageable)
            {
                targets.Add(damageable);
            }
        }
        
        private void OnTargetExit(Collider col, object target)
        {
            if (target is IDamageable damageable)
            {
                targets.Remove(damageable);
            }
        }
    }
}