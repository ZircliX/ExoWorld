using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Menus
{
    public class PlayerListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Button muteButton;
        
        public void Initialize(string playerName)
        {
            playerNameText.text = $"{playerName}";
        }
    }
}