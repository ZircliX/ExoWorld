using System;
using System.Collections.Generic;
using Helteix.Singletons.SceneServices;
 
 namespace OverBang.GameName.Gameplay
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
         
         
     }
 }