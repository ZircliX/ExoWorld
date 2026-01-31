using DG.Tweening;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.GameMode.Players;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

        [field : SerializeField] public GadgetData data { get; private set; }
        private GadgetControllerUI gadgetControllerUI;
        public bool isSelectable { get; private set; }
        public void Initialize(GadgetControllerUI ControllerUI)
        {
            gadgetControllerUI = ControllerUI;
        }

        public void Refresh(GadgetData Data, int gadgetAmount)
        {
            gameObject.SetActive(true);
            data = Data;
            icon.sprite = Data.Icon;
            icon.color = Color.white;
            gadgetTitle.text = Data.Name;
            gadgetDescription.text = Data.Description;
            amountTxt.text = gadgetAmount.ToString();
        }

        public void SelectThisGadget()
        {
            if (!isSelectable) return;
            HighLightBackGround(1f, Color.black);
        }

        public void DeselectThisGadget()
        {
            HighLightBackGround(0f, Color.white);
        }

        public void SetSelectable(bool selectability)
        {
            if (selectability)
            {
                isSelectable = true;
            }
            else
            {
                isSelectable = false;
            }
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