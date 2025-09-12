using LTX.ChanneledProperties.Priorities;
using OverBang.GameName.Metrics;
using UnityEngine;

namespace OverBang.GameName.Managers
{
    public static partial class GameController
    {
        public static GameMode GameMode { get; private set; }
        
        private static GameMetrics gameMetrics;
        public static GameMetrics Metrics
        {
            get
            {
                if (!gameMetrics)
                    gameMetrics = LoadMetrics();

                return gameMetrics;
            }
        }

        private static GameMetrics LoadMetrics() => Resources.Load<GameMetrics>("GameMetrics");
        
        public static Priority<CursorLockMode> CursorLockModePriority { get; private set; }
        public static Priority<bool> CursorVisibleStatePriority { get; private set; }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadGame()
        {
            SetupPrioritisedProperties();
            SetupFields();
        }

        public static void QuitGame()
        {
            
        }

        private static void SetupFields()
        {
            GameMode = new GameMode();
        }

        private static void SetupPrioritisedProperties()
        {
            CursorLockModePriority = new Priority<CursorLockMode>(CursorLockMode.None);
            CursorLockModePriority.AddOnValueChangeCallback(UpdateTimeScale, true);

            CursorVisibleStatePriority = new Priority<bool>(true);
            CursorVisibleStatePriority.AddOnValueChangeCallback(UpdateCursorVisibility, true);
        }

        private static void UpdateTimeScale(CursorLockMode value)
        {
            Cursor.lockState = value;
        }
        
        private static void UpdateCursorVisibility(bool value)
        {
            Cursor.visible = value;
        }
    }
}