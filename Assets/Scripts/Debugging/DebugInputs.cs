using OverBang.ExoWorld.Core;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Gameplay;
using OverBang.ExoWorld.Gameplay.Network;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Debugging
{
    public class DebugInputs : MonoBehaviour
    {
        [SerializeField] private CharacterData characterData;
        private static DebugInputs instance;

        private SurvivalGameMode mode;

        public SurvivalGameMode Mode
        {
            get
            {
                mode ??= mode.GetOrCreateGameMode(() => new SurvivalGameMode());
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

            mode = GameController.CurrentGameMode as SurvivalGameMode;
        }
        
        private void OnDestroy()
        {
            instance = null;
        }
        
        private void Update()
        {
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
    }
}