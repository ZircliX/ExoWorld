using System;
using OverBang.GameName.CharacterSelection;
using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Core.Services;
using OverBang.GameName.Gameplay.Gameplay.StateMachine;
using OverBang.GameName.Gameplay.States;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.GameName.Offline
{
    public class OfflineGameMode : IGameMode,
        ICharacterSelectionService,
        ICharacterSpawnService
    {
        public static OfflineGameMode Create(int map, int difficulty)
        {
            return new OfflineGameMode(map, difficulty);
        }

        public OfflineGameMode WithPlayer(PlayerProfile profile)
        {
            PlayerProfile = profile;
            return this;
        }

        public int Map { get; private set; }
        public int Difficulty { get; private set; }
        public PlayerProfile PlayerProfile { get; private set; }

        public readonly StateMachine<IGameState> StateMachine;
        private GameObject player;

        private OfflineGameMode(int map, int difficulty)
        {
            StateMachine = new StateMachine<IGameState>();
            
            Map = map;
            Difficulty = difficulty;
        }

        public void SetPlayerProfile(CharacterData character)
        {
            PlayerProfile profile = new PlayerProfile()
            {
                CharacterData = character,
                PlayerName = character.AgentName
            };
            SetPlayerProfile(profile);
        }
        
        public void SetPlayerProfile(PlayerProfile profile)
        {
            PlayerProfile = profile;
        }

        public void Activate()
        {
            Debug.Log("OfflineGameMode activated");
            
            if (!PlayerProfile.IsValid)
            {
                StateMachine.ChangeState(new CharacterSelectionState(StateMachine, this, this));
            }
        }

        public void Deactivate()
        {
            Debug.Log("OfflineGameMode deactivated");
            StateMachine.CurrentState?.Exit();
        }

        public void StartCharacterSelection(Action<CharacterData> onSelected)
        {
            CharacterSelectionManager.Instance.StartCharacterSelection(
                new CharacterSelectionManager.SelectionSettings
                {
                    Type = CharacterSelectionManager.SelectionSettings.SelectionType.Pick,
                    ClassLimitation = CharacterClasses.None
                },
                onSelected);
        }

        public void StopCharacterSelection(Action<CharacterData> onSelected)
        {
            CharacterSelectionManager.Instance.StopCharacterSelection(onSelected);
        }
        
        public void SetCharacter(CharacterData characterData)
        {
            SetPlayerProfile(characterData);
        }

        public void SpawnCharacter()
        {
            if (!PlayerProfile.IsValid) return;
            
            if (player != null)
            {
                Object.Destroy(player);
            }
            
            player = Object.Instantiate(PlayerProfile.CharacterData.CharacterPrefab);
        }
    }
}
