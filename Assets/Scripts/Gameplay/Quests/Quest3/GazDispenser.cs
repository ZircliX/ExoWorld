using System.Collections.Generic;
using Helteix.Tools;
using OverBang.ExoWorld.Gameplay.Abilities;
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

        private void Awake()
        {
            targets = new List<IDamageable>(4);
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
            timer += Time.deltaTime;
            if (timer >= SECOND)
            {
                timer = 0f;
                ApplyGazDamage();
            }
        }

        private void ApplyGazDamage()
        {
            buffer.CopyFrom(targets);
            for (int i = 0; i < buffer.Length; i++)
            {
                IDamageable target = buffer[i];
                target.TakeDamage(data.GazTickDamage);
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