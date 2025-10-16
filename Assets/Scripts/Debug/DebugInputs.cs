using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Managers;
using OverBang.GameName.Offline;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace OverBang.GameName.Debug
{
    public class DebugInputs : MonoBehaviour
    {
        [SerializeField] private CharacterData characterData;
        private static DebugInputs instance;

        private OfflineGameMode mode;

        public OfflineGameMode Mode
        {
            get
            {
                mode ??= mode.GetOrCreateGameMode(() => OfflineGameMode.Create(0, 0));
                return mode;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (instance == null)
            {
                Instantiate(GameController.Metrics.DebugInputs);
            }
        }
        
        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            mode = GameController.CurrentGameMode as OfflineGameMode;
        }
        
        private void OnDestroy()
        {
            instance = null;
        }
        
        private void Update()
        {
            // Return to Main Menu
            if (Keyboard.current.numpad0Key.wasPressedThisFrame)
            {
                SceneManager.LoadScene("MainMenu");
            }
            
            // Set PlayerProfile
            if (Keyboard.current.numpadPeriodKey.wasPressedThisFrame)
                Mode.SetPlayerProfile(new PlayerProfile()
                {
                    characterData = characterData,
                    playerName = characterData.AgentName
                });

            // Force Character Selection
            if (Keyboard.current.numpad1Key.wasPressedThisFrame)
            {
            }

            // Force Hub
            if (Keyboard.current.numpad2Key.wasPressedThisFrame)
            {
                //if (!Mode.PlayerProfile.IsValid) Mode.SetPlayerProfile(characterData);
            }

            // Force Gameplay
            if (Keyboard.current.numpad3Key.wasPressedThisFrame)
            {
                //if (!Mode.PlayerProfile.IsValid) Mode.SetPlayerProfile(characterData);
            }
        }

        private void OnStateChanged(IGameState newState)
        {
            UnityEngine.Debug.Log($"[DebugInputs] State changed to: {newState.Name}");
        }
    }
}