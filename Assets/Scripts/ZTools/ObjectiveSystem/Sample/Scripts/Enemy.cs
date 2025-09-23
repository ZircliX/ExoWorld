using UnityEngine;
using ZTools.ObjectiveSystem.Core;
using ZTools.ObjectiveSystem.Sample.Events;

namespace ZTools.ObjectiveSystem.Sample.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private string enemyTag = "Enemy";
        
        private void Update()
        {
            // Simulates killing the enemy by pressing the space key
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // !!! This line is the key part of the example !!!
                ObjectivesManager.DispatchGameEvent(new KillEnemyEvent(enemyTag, 1));
            }
        }
    }
}