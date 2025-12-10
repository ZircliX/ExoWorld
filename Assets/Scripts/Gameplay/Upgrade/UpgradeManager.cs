using System;
using System.Collections.Generic;
using Helteix.Singletons.MonoSingletons;
using OverBang.GameName.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class UpgradeManager : MonoSingleton<UpgradeManager>
    {
        private Dictionary<UpgradeType, Upgrade> upgrades;
        private Dictionary<UpgradeType, RuntimeUpgradeData> playerUpgradesDatas;
        
        public event Action<int> OnPlayerTritiniteAmountChange;


        private void OnEnable()
        {
            upgrades = new Dictionary<UpgradeType, Upgrade>();
            
        }
        
        public void InitializeUpgrades(IPlayer player)
        {
            
            playerUpgradesDatas = new Dictionary<UpgradeType, RuntimeUpgradeData>(4);
            
            if (player.TryGetCharacterDataByPlayer(out CharacterData characterData))
            {
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
            else
            {
                Debug.LogError("Could not find CharacterData !!");
            }
            
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
                Debug.Log($"{runtimeData.upgradeData.UpgradeName}, {runtimeData.finalBonus}");

                Upgrade upgrade = upgrades[runtimeData.upgradeData.UpgradeType];
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
                    if (PlayerInventory.Trinitite >= runtimeData.cost)
                    {
                        int newtriniAmount = PlayerInventory.DecrementTrinitite(runtimeData.cost);
                        OnPlayerTritiniteAmountChange?.Invoke(newtriniAmount);
                        
                        int newLevel = runtimeData.level + 1 ;
                        
                        float newBonus = runtimeData.finalBonus + runtimeData.bonus * newLevel ;
                        
                        int newCost = runtimeData.initialCost * newLevel;
                        
                        runtimeData.SetValue(newBonus, newLevel, newCost);
                        playerUpgradesDatas[data.Key] = runtimeData;
                        
                        upgrade.ui.Refresh(runtimeData);
                        
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