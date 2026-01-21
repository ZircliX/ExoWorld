using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core
{
    public class ContactsUI : NavigablePanel
    {
        [SerializeField] private Button saveButton;
        [SerializeField] private TMP_InputField playerNameInput;

        protected override void Awake()
        {
            base.Awake();
            saveButton.onClick.AddListener( () =>
            {
                string playerNameKey = GameMetrics.Global.ConstID.PlayerPropertyPlayerName;
                PlayerPrefs.SetString(playerNameKey, playerNameInput.text);
                OnBackClicked?.Invoke();
            });
        }

        protected override void OnShow()
        {
            string playerNameKey = GameMetrics.Global.ConstID.PlayerPropertyPlayerName;
            if (PlayerPrefs.HasKey(playerNameKey))
            {
                playerNameInput.text = PlayerPrefs.GetString(playerNameKey);
            }
        }
    }
}