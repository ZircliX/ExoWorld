using System.Collections.Generic;
using Helteix.Singletons.SceneServices;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
 {
     public class EnemyManager : SceneService<EnemyManager>
     {
        private List<Enemy> enemies;

        private void Start()
        {
            enemies = new List<Enemy>();
        }

        public void Register(Enemy enemy)
         {
             enemies.Add(enemy);
         }

         public void Unregister(Enemy enemy)
         {
             enemies.Remove(enemy);
         }

         public bool TryGetClosest(Vector3 position, out ITargetable closest)
         {
             closest = null;
             
             if (enemies.Count == 0)
                 return false;
             
             float closestDistance = Vector3.Distance(position, enemies[0].transform.position);
             foreach (Enemy enemy in enemies)
             {
                 float distance = Vector3.Distance(position, enemy.transform.position);
                 if (distance < closestDistance)
                 {
                     closestDistance = distance;
                     closest = enemy;
                 }
             }

             return true;
         }
     }
 }