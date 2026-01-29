using OverBang.ExoWorld.Core.Abilities.Gadgets;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class GadgetUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;

        private GadgetControllerUI gadgetControllerUI;
        private GadgetData data;
        
        public void Initialize(GadgetControllerUI ControllerUI, GadgetData Data)
        {
            gadgetControllerUI = ControllerUI;
            data = Data;
            
            image.sprite = Data.Icon;
            text.text = Data.Name;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            gadgetControllerUI.SetCurrentSelectedGadget(data);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gadgetControllerUI.SetCurrentSelectedGadget(null);
        }
        
        public void HightLight()
        {
            
        }

    }
}