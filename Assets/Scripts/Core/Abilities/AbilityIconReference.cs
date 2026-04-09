using DG.Tweening;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace OverBang.ExoWorld.Core.Abilities
{
    public class AbilityIconReference : MonoBehaviour
    {
        [field: SerializeField] public Image IconOutline { get; private set; }
        [field: SerializeField] public Image CooldownFill { get; private set; }
        [field: SerializeField] public Image AbilityIcon { get; private set; }
        [field: SerializeField] public TMP_Text CooldownText { get; private set; }
        [field: SerializeField] public Image AbilityHighlight { get; private set; }

        [Space] 
        [SerializeField] private Color orange;
        [SerializeField] private Color black;
        [SerializeField] private Color white;

        public void Begin(float duration)
        {
            IconOutline.fillAmount = 1;
            IconOutline.DOFillAmount(0, duration).SetEase(Ease.Linear);
        }

        public void End(float cooldown)
        {
            IconOutline.fillAmount = 1;
            CooldownFill.color = black;
            
            AbilityIcon.fillAmount = 0;
            AbilityIcon.DOFillAmount(1, cooldown).SetEase(Ease.Linear);

            CooldownText.GetComponent<CanvasGroup>().alpha = 1;
            CooldownText.text = Mathf.CeilToInt(cooldown).ToString();
            
            float textValue = cooldown;
            DOTween.To(() => textValue, x =>
                {
                    textValue = x;
                    CooldownText.text = Mathf.CeilToInt(x).ToString();
                },
                0f,
                cooldown)
                .SetEase(Ease.Linear);
        }

        public void CooldownEnd()
        {
            CooldownFill.color = white;
            CooldownText.GetComponent<CanvasGroup>().alpha = 0;
            AbilityIcon.fillAmount = 0;
            AbilityHighlight.DOFade(1, 0.2f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);
            transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);
        }
    }
}