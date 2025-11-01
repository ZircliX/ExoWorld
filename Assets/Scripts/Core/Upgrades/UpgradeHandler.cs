using System;
using UnityEngine;

namespace OverBang.GameName.Core.Upgrades
{
    public class UpgradeHandler : MonoBehaviour
    {
        [SerializeField] private Upgrade[] availableUpgrades;

        private void Start()
        {
            for (int i = 0; i < availableUpgrades.Length; i++)
            {
                Debug.Log($"Applying upgrade : {availableUpgrades[i].name}");
                availableUpgrades[i].ApplyUpgrade();
            }
        }
    }
}