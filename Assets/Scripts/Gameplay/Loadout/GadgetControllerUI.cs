using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class GadgetControllerUI : MonoBehaviour
    {
        [SerializeField, Required] private GadgetController controller;
        
        [SerializeField] private CanvasGroup gadgetWheel;


        private void OnEnable()
        {
            controller.OnGadgetSelectionBegin += SelectionBegin;
            controller.OnGadgetSelectionEnd += SelectionEnd;
        }

        private void OnDisable()
        {
            controller.OnGadgetSelectionBegin -= SelectionBegin;
            controller.OnGadgetSelectionEnd -= SelectionEnd;
        }

        private void SelectionBegin()
        {
            ChangeVisibility(true);
            
            
        }
        
        private void SelectionEnd()
        {
            ChangeVisibility(false);
            
        }
        
        private void ChangeVisibility(bool visible)
        {
            gadgetWheel.DOFade(visible ? 1 : 0, 0.20f).OnComplete(() =>
            {
                gadgetWheel.interactable = visible;
                gadgetWheel.blocksRaycasts = visible;
            });
        }
        
        
    }
}