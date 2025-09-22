using System;
using LTX.ChanneledProperties.Priorities;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Core.Metrics;
using UnityEngine;

namespace OverBang.GameName.Managers
{
    public static partial class GameController
    {
        public static IGameMode CurrentGameMode { get; private set; }
        
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

        public static void SetGameMode(this IGameMode mode)
        {
            CurrentGameMode?.Deactivate();

            CurrentGameMode = mode;
            CurrentGameMode.Activate();
        }
        
        public static T GetOrCreateGameMode<T>(this IGameMode mode, Func<T> factory) 
            where T : class, IGameMode
        {
            if (CurrentGameMode is T existing)
                return existing;

            T newMode = factory();
            newMode.SetGameMode();
            return newMode;
        }
    }
}