using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Player.PlayerHUD
{
    public class TeammateInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private Image characterIcon;
        [SerializeField] private Image healthBar;
        [SerializeField] private Image healthBarBg;
        
        public void OnHealthChanged(float health, float maxHealth)
        {
            healthBar.fillAmount = health / maxHealth;
            healthBarBg.DOFillAmount(health / maxHealth, 0.2f);
        }

        public void SetInfos(string pName, Sprite icon)
        {
            playerName.text = pName;
            characterIcon.sprite = icon;
        }
    }
}