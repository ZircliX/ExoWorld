using TMPro;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class PumpUI : MonoBehaviour
    {
        [SerializeField] private Pump pump;
        [SerializeField] private TMP_Text progressText;

        private void Start()
        {
            progressText.text = string.Empty;
        }

        private void OnEnable()
        {
            pump.OnProgressChanged += RefreshProgress;
        }

        private void OnDisable()
        {
            pump.OnProgressChanged -= RefreshProgress;
        }

        private void RefreshProgress(float current, float max)
        {
            progressText.text = $"{current}/{max}";
        }
    }
}