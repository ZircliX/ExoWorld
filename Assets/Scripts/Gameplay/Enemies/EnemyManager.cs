using System;
using System.Collections.Generic;
using Helteix.Singletons.SceneServices;
using OverBang.ExoWorld.Gameplay.Targeting;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Enemies
{
    public class EnemyManager : SceneService<EnemyManager>
    {
        private readonly int maxEnemyCount = 12;

        private List<Enemy> enemies;
        public IReadOnlyList<Enemy> Enemies => enemies;

        public event Action<Enemy> OnEnemyRegistered;
        public event Action<Enemy> OnEnemyUnregistered;

        private void Start()
        {
            enemies = new List<Enemy>();
        }

        public void Register(Enemy enemy)
        {
            if (enemies.Count >= maxEnemyCount)
            {
                Debug.LogWarning("Cannot register enemy: max enemy count reached.");
                enemy.GetComponent<NetworkObject>().Despawn();
                return;
            }
            
            enemies.Add(enemy);
            OnEnemyRegistered?.Invoke(enemy);
        }

        public void Unregister(Enemy enemy)
        {
            enemies.Remove(enemy);
            OnEnemyUnregistered?.Invoke(enemy);
        }

        public bool TryGetClosest(Vector3 position, float maxDistance, out ITargetable closest)
        {
            closest = null;

            if (enemies.Count == 0)
                return false;

            float closestDistance = float.MaxValue;

            for (int index = 0; index < enemies.Count; index++)
            {
                Enemy enemy = enemies[index];
                if (!enemy.IsTargetable)
                    continue;

                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance < closestDistance && distance <= maxDistance)
                {
                    closestDistance = distance;
                    closest = enemy;
                }
            }

            return closest != null;
        }
    }
}