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
        private List<RuntimeUpgradeData> playerUpgradesDatas;

        
        private void OnEnable()
        {
            upgrades = new Dictionary<UpgradeType, Upgrade>();
            
        }
        
        public void InitializeUpgrades(IPlayer player)
        {
            
            playerUpgradesDatas = new List<RuntimeUpgradeData>(4);
            
            if (player.TryGetCharacterDataByPlayer(out CharacterData characterData))
            {
                foreach (UpgradeData upgrade in characterData.UpgradeDatas)
                {
                    RuntimeUpgradeData run = new RuntimeUpgradeData()
                    {
                        upgradeData = upgrade,
                    };
                    run.Initialize();
                    playerUpgradesDatas.Add(run);
                }

                RefreshTable();
            }
            else
            {
                Debug.LogError("Could not find CharacterData !!");
            }
            
        }

        public void RefreshTable()
        {
            foreach (RuntimeUpgradeData runtimeUpgradeData in playerUpgradesDatas)
            {
                Upgrade upgrade = upgrades[runtimeUpgradeData.upgradeData.UpgradeType];
                upgrade.ui.Refresh(runtimeUpgradeData);
            }
        }
        
        public bool TryToUpgrade(UpgradeType type, Upgrade upgrade)
        {
            foreach (RuntimeUpgradeData data in playerUpgradesDatas)
            {
                if (data.upgradeData.UpgradeType == type)
                {
                        Debug.Log($"Before Upgrade for {data.upgradeData.name} : level : {data.level} : cost :  {data.cost} : bonus :  {data.bonus} ");
                    if (PlayerInventory.Trinitite >= data.cost)
                    {
                        PlayerInventory.DecrementTrinitite(data.cost);
                        Debug.Log(data.level);
                        int newLevel = data.level + 1 ;
                        Debug.Log($"New Level: {newLevel}");
                        float newBonus = data.finalBonus + data.bonus * newLevel ;
                        Debug.Log(newBonus);
                        int newCost = data.cost * newLevel;
                        
                        data.SetValue(newBonus, newLevel, newCost);
                        
                        upgrade.ui.Refresh(data);
                        
                        Debug.Log($"Upgrade Complete for {data.upgradeData.name} : level : {data.level} : cost :  {data.cost} : bonus :  {data.bonus}");
                        return true;
                    }
                    else
                    {
                        Debug.Log($"not enough Trinitite, Upgrade Cost : {data.cost}, need {data.cost - PlayerInventory.Trinitite} more !!");
                
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
        
        #endregion Register / unregister upgrades
        
    }
}