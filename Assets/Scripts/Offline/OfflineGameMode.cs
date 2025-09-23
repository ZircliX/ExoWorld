using OverBang.GameName.CharacterSelection;
using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.GameMode;
using UnityEngine;

namespace OverBang.GameName.Offline
{
    public class OfflineGameMode : IGameMode
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

        private GameObject player;

        private OfflineGameMode(int map, int difficulty)
        {
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

        public async Awaitable Run()
        {
            if (!PlayerProfile.IsValid)
            {
                CharacterData newCharacter = await HubPhase.CreateAsync(new HubPhase.SelectionSettings
                {
                    type = HubPhase.SelectionType.Pick,
                    availableClasses = CharacterClasses.All,
                    gameDatabase = GameController.GameDatabase
                });
                
                SetPlayerProfile(newCharacter);
            }
        }
    }
}