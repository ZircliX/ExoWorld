using DG.Tweening;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class GadgetUi : MonoBehaviour
    {
        [field : SerializeField] public bool isSelectable { get; private set; }
        [SerializeField] private Image icon;
        [SerializeField] private Image iconBackGround;
        [SerializeField] private TMP_Text amountTxt;
        [SerializeField] private TMP_Text gadgetTitle;
        [SerializeField] private TMP_Text gadgetDescription;
        [SerializeField] private Color maskColor;
        public GadgetData data { get; private set; }
        private int amount;
        private GadgetControllerUI gadgetControllerUI;
        
       

        public void Refresh(GadgetData Data, int gadgetAmount)
        {
            gameObject.SetActive(true);
            data = Data;
            amount = gadgetAmount;
            icon.sprite = Data.Icon;
            icon.color = Color.white;
            amountTxt.color =  Color.white;
            gadgetTitle.text = Data.Name;
            gadgetDescription.text = Data.Description;
            amountTxt.text = gadgetAmount.ToString();
        }

        public void SelectThisGadget()
        {
            if (!isSelectable) return;
            Refresh(data,amount);
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

        public void Mask()
        {
            icon.color = maskColor;
            amountTxt.color = maskColor;
        }

        public bool CheckSelectiveness()
        {
            return isSelectable;
        }
        private void HighLightBackGround(float visibility, Color color)
        {
            iconBackGround.DOKill();
            icon.DOKill();
            
            Refresh(data,amount);
            iconBackGround.DOFade(visibility, 0.15f);
            icon.DOColor(color, 0.15f);
            
        }

    }
}