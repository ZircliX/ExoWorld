using DG.Tweening;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtils;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class GadgetUi : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image iconBackGround;
        [SerializeField] private TMP_Text amountTxt;
        [SerializeField] private TMP_Text gadgetTitle;
        [SerializeField] private TMP_Text gadgetDescription;
        [SerializeField] private int amount;

        [field : SerializeField] public GadgetData data { get; private set; }
        private GadgetControllerUI gadgetControllerUI;
        
        public void Initialize(GadgetControllerUI ControllerUI)
        {
            gadgetControllerUI = ControllerUI;
        }

        public void Refresh(GadgetData Data)
        {
            gameObject.SetActive(true);
            data = Data;
            icon.sprite = Data.Icon;
            icon.color = Color.white;
            iconBackGround.color.SetAlpha(1.0f);
            gadgetTitle.text = Data.Name;
            gadgetDescription.text = Data.Description;
        }

        public void SelectThisGadget()
        {
            HighLightBackGround(1f, Color.black);
            gadgetControllerUI.SetCurrentSelectedGadget(data);
        }

        public void DeselectThisGadget()
        {
            gadgetControllerUI.SetCurrentSelectedGadget(null);
            HighLightBackGround(0f, Color.white);
        }

        public void Clear()
        {
            icon.sprite = null;
            iconBackGround.sprite = null;
            gadgetTitle.text = string.Empty;
            gadgetDescription.text = string.Empty;
            gameObject.SetActive(false);
        }
        
        private void HighLightBackGround(float visibility, Color color)
        {
            iconBackGround.DOFade(visibility, 0.15f);
            icon.DOColor(color, 0.15f);
        }

    }
}