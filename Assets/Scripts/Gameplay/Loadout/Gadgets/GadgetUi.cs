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
        private IGadget gadget;
        
        public void Initialize(GadgetControllerUI gadgetControllerUI, IGadget gadget)
        {
            this.gadgetControllerUI = gadgetControllerUI;
            this.gadget = gadget;
            
            image.sprite = gadget.Data.Icon;
            text.text = gadget.Data.Name;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            gadgetControllerUI.SetCurrentSelectedGadget(gadget);
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