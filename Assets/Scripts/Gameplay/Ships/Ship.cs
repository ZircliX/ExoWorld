using OverBang.GameName.Core.Phases;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Ships
{
    public class Ship : MonoPhaseListener<GameplayPhase>
    {
        [SerializeField] private GameObject go;
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Entered Ship");
            if (other.CompareTag("Player") && go.activeInHierarchy)
            {
                Debug.Log("Ending hub phase");
                _ = currentPhase.End(true);
            }
        }
    }
}