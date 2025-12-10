using OverBang.GameName.Core;
using TMPro;
using UnityEngine;

namespace OverBang.GameName.Gameplay
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
                
            }
            else
            {
                
            }
        }
    }
}