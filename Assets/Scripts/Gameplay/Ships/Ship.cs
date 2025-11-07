using OverBang.GameName.Core.Phases;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Ships
{
    public class Ship : MonoPhaseListener<GameplayPhase>
    {
        [SerializeField] private GameObject go;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && go.activeInHierarchy)
            {
                _ = currentPhase.End(true);
            }
        }
    }
}