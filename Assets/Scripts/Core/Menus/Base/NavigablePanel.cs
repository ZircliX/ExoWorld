using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.GameName.Core.Menus
{
    public abstract class NavigablePanel : BasePanel, INavigablePanel
    {
        [SerializeField] protected Button backButton;

        public event Action OnBackClicked;

        protected override void Awake()
        {
            base.Awake();
            if (backButton != null)
                backButton.onClick.AddListener(InvokeBackClicked);
        }

        public void InvokeBackClicked()
        {
            OnBackClicked?.Invoke();
        }
    }
}