using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.GameName.Core.Menus
{
    public class HostGameUI : InteractivePanel
    {
        [SerializeField] private TMP_InputField gameNameInput;
        [SerializeField] private Button createButton;

        public event Action OnHostCreated;

        protected override void Awake()
        {
            base.Awake();
            OnBackClicked += Hide;
            createButton.onClick.AddListener(() => HandleHostCreate(gameNameInput.text));
        }

        private void HandleHostCreate(string gameName)
        {
            OnHostCreated?.Invoke();
        }
    }
}