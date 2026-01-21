using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core
{
    public abstract class NavigablePanel : BasePanel, INavigablePanel
    {
        [SerializeField] protected Button backButton;

        public Action OnBackClicked { get; protected internal set; }

        protected override void Awake()
        {
            base.Awake();
            if (backButton != null)
                backButton.onClick.AddListener(InvokeBackClicked);
        }

        public virtual void InvokeBackClicked()
        {
            OnBackClicked?.Invoke();
        }
    }
}