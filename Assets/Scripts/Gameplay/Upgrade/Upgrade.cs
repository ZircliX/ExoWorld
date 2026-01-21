using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    public class Upgrade : MonoBehaviour
    {
        [field : SerializeField] public UpgradeUi ui {get; private set;}
        [field : SerializeField] public UpgradeType UpgradeType {get; private set;}

        private void OnEnable()
        {
            UpgradeManager.Instance.RegisterUpgrade(this);
        }
        private void OnDisable()
        {
            UpgradeManager.Instance.UnregisterUpgrade(this);
        }
        
        public void OnTryToUpgrade()
        {
            if (UpgradeManager.Instance.TryToUpgrade(UpgradeType, this))
            {
                Debug.Log($"{UpgradeType} upgraded!");
            }
            else
            {
                Debug.Log("Upgrade failed! Level max or not enough !");
            }
        }
    }
}