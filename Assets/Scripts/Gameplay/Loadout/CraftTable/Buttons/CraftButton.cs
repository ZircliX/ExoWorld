using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public abstract class CraftButton : MonoBehaviour
    {
        [SerializeField, Self] protected Button button;
        protected CraftTableUI craftTableUI;
        protected TMP_Text text;

        protected virtual void OnValidate()
        {
            this.ValidateRefs();
        }

        private void Awake()
        {
            text = transform.GetChild(0).GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClicked);
            Enable();
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClicked);
            Disable();
        }

        protected abstract void Enable();
        protected abstract void Disable();

        protected abstract void OnButtonClicked();
        public abstract void Initialize(CraftTableUI ui);
    }
}