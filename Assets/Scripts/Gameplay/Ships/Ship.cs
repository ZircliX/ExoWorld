using OverBang.GameName.Gameplay.Gameplay.Listeners;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Ships
{
    public class Ship : GameplayListener
    {
        [SerializeField] private GameObject go;
        
        protected override void Initialize(GameplayPhase phase)
        {
        }

        protected override void Release(GameplayPhase phase)
        {
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && go.activeInHierarchy)
            {
                current.CompletePhase(true);
            }
        }
    }
}