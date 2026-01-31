using System;
using System.Collections.Generic;
using Helteix.Singletons.MonoSingletons;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Inventory;
using OverBang.ExoWorld.Core.Upgrade;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Upgrade
{
    public class UpgradeManager : MonoSingleton<UpgradeManager>
    {
        private Dictionary<UpgradeType, Upgrade> upgrades;
        
        public Dictionary<UpgradeType, RuntimeUpgradeData> playerUpgradesDatas { get; private set; }
        
        public event Action<int> OnPlayerTritiniteAmountChange;
        public event Action OnUpgrade;
        
        private void OnEnable()
        {
            upgrades = new Dictionary<UpgradeType, Upgrade>();
        }

        public void InitializeUpgrades(IGamePlayer player)
        {
            playerUpgradesDatas = new Dictionary<UpgradeType, RuntimeUpgradeData>(4);
            CharacterData characterData = player.CharacterData;
            Debug.Log($"Data Loaded with {playerUpgradesDatas.Count} upgrades");
            
            foreach (UpgradeData upgrade in characterData.UpgradeDatas)
            {
                RuntimeUpgradeData run = new RuntimeUpgradeData()
                {
                    upgradeData = upgrade,
                };
                run.Initialize();
                playerUpgradesDatas.Add(run.upgradeData.UpgradeType, run);
            }

            RefreshTable();
            
        }

        public float GetRuntimeUpgrade(UpgradeType type)
        {
            RuntimeUpgradeData data =  playerUpgradesDatas[type];
            
            return data.finalBonus;
        }

        public void RefreshTable()
        {
            foreach (KeyValuePair<UpgradeType, RuntimeUpgradeData> runtimeUpgradeData in playerUpgradesDatas)
            {
                RuntimeUpgradeData runtimeData = runtimeUpgradeData.Value;

                Upgrade upgrade = upgrades[runtimeUpgradeData.Key];
                
                upgrade.ui.Refresh(runtimeData);
            }
        }
        
        public bool TryToUpgrade(UpgradeType type, Upgrade upgrade)
        {
            foreach (KeyValuePair<UpgradeType, RuntimeUpgradeData> data in playerUpgradesDatas)
            {
                RuntimeUpgradeData  runtimeData = data.Value;
                if (runtimeData.upgradeData.UpgradeType == type)
                {
                    if (ResourcesInventory.Instance.GetItemQuantity("Trinitite") >= runtimeData.cost && runtimeData.level < 3)
                    {
                        int newTrinititeAmount = ResourcesInventory.Instance.RemoveItem("Trinitite", runtimeData.cost);
                        OnPlayerTritiniteAmountChange?.Invoke(newTrinititeAmount);
                        
                        int newLevel = runtimeData.level + 1;
                        
                        float newBonus = runtimeData.finalBonus + runtimeData.bonus * newLevel;
                        
                        int newCost = runtimeData.initialCost * newLevel;
                        
                        runtimeData.SetValue(newBonus, newLevel, newCost);
                        playerUpgradesDatas[data.Key] = runtimeData;
                        
                        upgrade.ui.Refresh(runtimeData);
                        
                        OnUpgrade?.Invoke();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }
        
        
        #region Register / unregister upgrades
        
        public void RegisterUpgrade(Upgrade upgrade)
        {
            upgrades.Add(upgrade.UpgradeType, upgrade);
        }

        public void UnregisterUpgrade(Upgrade upgrade)
        {
            upgrades.Remove(upgrade.UpgradeType);
        }
        
        #endregion
        
    }
}