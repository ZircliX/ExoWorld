using System;
using System.Collections.Generic;
using Helteix.Singletons.SceneServices;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
 {
     public class EnemyManager : SceneService<EnemyManager>
     {
        private List<Enemy> enemies;

        public event Action<Enemy> OnEnemyRegistered; 
        public event Action<Enemy> OnEnemyUnregistered; 

        private void Start()
        {
            enemies = new List<Enemy>();
        }

        public void Register(Enemy enemy)
         {
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
             
             float closestDistance = Vector3.Distance(position, enemies[0].transform.position);
             closest = enemies[0];
             
             for (int index = 1; index < enemies.Count; index++)
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

             return true;
         }
     }
 }