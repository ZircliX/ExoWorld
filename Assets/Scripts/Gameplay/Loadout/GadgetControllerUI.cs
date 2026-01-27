using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    public class GadgetControllerUI : MonoBehaviour
    {
        [SerializeField, Required] private GadgetController controller;
        
        [SerializeField] private CanvasGroup gadgetWheel;
        [SerializeField] private float radius = 250f;
        [SerializeField] private float startAngle;
        [SerializeField] private GadgetUi gadgetUiPrefab;
        
        private List<GadgetUi> menuItems;
        
        private void Start()
        {
            menuItems  = new List<GadgetUi>();
            gadgetWheel.alpha = 0;
        }

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
        
        public void SetCurrentSelectedGadget(IGadget gadget)
        {
            controller.SelectCurrentGadget(gadget);
        }
        
        private void ChangeVisibility(bool visible)
        {
            gadgetWheel.DOFade(visible ? 1 : 0, 0.20f).OnComplete(() =>
            {
                gadgetWheel.interactable = visible;
                gadgetWheel.blocksRaycasts = visible;
            });
        }
        
        public void AddGadgetToWheel(IGadget gadget)
        {
            GadgetUi gadgetUi = Instantiate(gadgetUiPrefab, transform);
            gadgetUi.Initialize(this, gadget);
        
            menuItems.Add(gadgetUi);
            RepositionItems();
        }

        private void RepositionItems()
        {
            int itemCount = menuItems.Count;
            if (itemCount == 0) return;

            float angleStep = 360f / itemCount;

            for (int i = 0; i < itemCount; i++)
            {
                float angle = (startAngle + angleStep * i) * Mathf.Deg2Rad;
            
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                menuItems[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            }
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < menuItems.Count)
            {
                Destroy(menuItems[index]);
                menuItems.RemoveAt(index);
                RepositionItems();
            }
        }

        public void ClearMenu()
        {
            foreach (GadgetUi item in menuItems)
            {
                Destroy(item);
            }
            menuItems.Clear();
        }
    }
}