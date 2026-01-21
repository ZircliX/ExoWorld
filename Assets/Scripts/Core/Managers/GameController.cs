using System;
using Helteix.ChanneledProperties.Priorities;
using UnityEngine;

namespace OverBang.ExoWorld.Core
{
    public static partial class GameController
    {
        public static IGameMode CurrentGameMode { get; private set; }
        public static SessionManager SessionManager { get; private set; }

        private static GameDatabase gameDatabase;
        public static GameDatabase GameDatabase
        {
            get
            {
                if (!gameDatabase)
                    gameDatabase = LoadGameDatabase();
                
                return gameDatabase;
            }
        }
        
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
        private static GameDatabase LoadGameDatabase() => Resources.Load<GameDatabase>("GameDatabase");
        
        public static Priority<CursorLockMode> CursorLockModePriority { get; private set; }
        public static Priority<bool> CursorVisibleStatePriority { get; private set; }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadGame()
        {
            SetupPrioritisedProperties();
            SetupFields();

            Application.targetFrameRate = 60;
        }

        public static void QuitGame()
        {
            
        }

        private static void SetupFields()
        {
            SessionManager = new SessionManager();
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
            CurrentGameMode = mode;
            Awaitable result = CurrentGameMode.Run();
            result.Run();
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