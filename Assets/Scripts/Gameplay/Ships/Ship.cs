using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
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