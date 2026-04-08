using System;
using System.Collections.Generic;
using System.Globalization;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.Inventory;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Utils;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace OverBang.ExoWorld.Core.GameMode.Players
{
    public class LocalGamePlayer : IGamePlayer
    {
        public CharacterData CharacterData { get; private set;}

        public float Health { get; private set; } = -1;
        public float MaxHealth { get; private set; } = -1;
        public event Action<float, float> OnHealthChanged;
        public PlayerState State { get; private set; } = PlayerState.Uninitialized;
        public event Action<PlayerState> OnStateChanged; 

        public string SessionPlayerID => LocalSessionPlayer.Id;
        public ulong ClientID => NetworkManager.Singleton.LocalClientId;
        public bool IsHost => NetworkManager.Singleton.LocalClientId == 0;
        
        private IPlayer LocalSessionPlayer => SessionManager.Global.CurrentPlayer;
        public IReadOnlyPlayer SessionPlayer => LocalSessionPlayer;
        public NetworkObject CurrentPlayerObject { get; private set; }
        
        public ResourcesInventory Inventory { get; private set; } = new ResourcesInventory();
        public GadgetInventory GadgetInventory {get; private set;} = new GadgetInventory();
        
        private bool isDirty;
        private float lastSaveTime = float.MinValue;
        private const float SAVE_COOLDOWN = 1.5f;

        public LocalGamePlayer()
        {
            ScriptableItemData[] resources = Resources.LoadAll<ScriptableItemData>("ItemData");

            for (int i = 0; i < resources.Length; i++)
            {
                ScriptableItemData itemData = resources[i];
                Inventory.AddItem(itemData.Data, itemData.DefaultQuantity);
            }
        }
        
        /// <summary>
        /// Called every x frame to apply changes to the player properties
        /// </summary>
        public void ApplyIfDirty()
        {
            if (!isDirty) 
                return;
            if (Time.realtimeSinceStartup - lastSaveTime < SAVE_COOLDOWN) 
                return;
            
            using (DictionaryPool<string, PlayerProperty>.Get(out Dictionary<string, PlayerProperty> properties))
            {
                properties[ConstID.Global.PlayerPropertyHealth] = new PlayerProperty(Health.ToString(CultureInfo.InvariantCulture), VisibilityPropertyOptions.Public);
                properties[ConstID.Global.PlayerPropertyMaxHealth] = new PlayerProperty(MaxHealth.ToString(CultureInfo.InvariantCulture), VisibilityPropertyOptions.Public);
                properties[ConstID.Global.PlayerPropertyState] = new PlayerProperty(State.ToString(), VisibilityPropertyOptions.Public);
                
                properties[ConstID.Global.PlayerPropertyCharacterData] = new PlayerProperty(CharacterData.ID, VisibilityPropertyOptions.Public);
                properties[ConstID.Global.PlayerPropertyClientID] = new PlayerProperty(ClientID.ToString(), VisibilityPropertyOptions.Public);
                properties[ConstID.Global.PlayerPropertyHost] = new PlayerProperty(IsHost.ToString(), VisibilityPropertyOptions.Public);
                    
                LocalSessionPlayer.SetProperties(properties);
            }

            SavePlayerData().Run();
            isDirty = false;
            lastSaveTime = Time.realtimeSinceStartup;
        }

        private async Awaitable SavePlayerData()
        {
            try
            {
                await SessionManager.Global.ActiveSession.SaveCurrentPlayerDataAsync();
            }
            catch (Exception e)
            {
                await Awaitable.MainThreadAsync();
                Debug.LogException(e);
            }
        }
        public void SetCharacterData(CharacterData characterData)
        {
            CharacterData = characterData;
            MaxHealth = characterData.BaseStats.Health;
            Health = MaxHealth;
            
            isDirty = true;
        }

        public void SetHealth(float newHealth)
        {
            Health = newHealth;
            isDirty = true;
            OnHealthChanged?.Invoke(Health, MaxHealth);
        }

        public void SetMaxHealth(float newMaxHealth)
        {
            MaxHealth = newMaxHealth;
            isDirty = true;
            OnHealthChanged?.Invoke(Health, MaxHealth);
        }
        
        public void SetState(PlayerState newState)
        {
            State = newState;
            isDirty = true;
            OnStateChanged?.Invoke(State);
        }

        public NetworkObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            NetworkObject playerObject = Object.Instantiate(GameMetrics.Global.PlayerControllerPrefab, position, rotation);
            playerObject.SpawnAsPlayerObject(ClientID, destroyWithScene: true);
            
            //Debug.Log($"Instantiated player object {playerObject.name}", playerObject);
            if (playerObject.TryGetComponent(out IPlayerController playerController))
            {
                playerController.Connect(this);
            }
            
            CurrentPlayerObject = playerObject;
            return playerObject;
        }
        
    }
}