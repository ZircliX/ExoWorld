using OverBang.ExoWorld.Gameplay.Level;
using TMPro;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Player.PlayerHUD
{
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;

        private void OnEnable()
        {
            if (LevelManager.Instance != null)
                LevelManager.Instance.OnTimerTick += OnTimerTick;
            else
                LevelManager.OnLevelManagerCreated += OnCreated;
        }

        private void OnDisable()
        {
            if (LevelManager.Instance != null)
                LevelManager.Instance.OnTimerTick -= OnTimerTick;
        }

        private void OnCreated()
        {
            LevelManager.OnLevelManagerCreated -= OnCreated;
            LevelManager.Instance.OnTimerTick += OnTimerTick;
        }
        
        private void OnTimerTick(float currentTime)
        {
            timerText.text = $"{Mathf.FloorToInt(currentTime / 60):00}:{Mathf.FloorToInt(currentTime % 60):00}";
        }
    }
}