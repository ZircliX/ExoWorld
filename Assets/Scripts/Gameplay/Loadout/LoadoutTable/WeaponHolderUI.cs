using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class WeaponHolderUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button button;
        [field: SerializeField] public  WeaponData Data { get; private set; }
        [SerializeField] private GameObject weapon;
        
        [Space]
        [SerializeField] private float hoverDistance = 0.3f;
        [SerializeField] private float hoverDuration = 0.5f;

        private LoadoutTableUI loadoutTableUI;
        private Vector3 originalPosition;
        private bool isSelected;

        private void Awake()
        {
            originalPosition = weapon.transform.localPosition;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        public void Initialize(LoadoutTableUI loadoutTableUI)
        {
            this.loadoutTableUI = loadoutTableUI;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsInteractable()) 
                return;
            
            weapon.transform.DOKill();
            weapon.transform.DOLocalMove(
                originalPosition + Vector3.right * hoverDistance, hoverDuration).SetEase(Ease.OutQuad);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsInteractable()) 
                return;
            
            weapon.transform.DOKill();
            weapon.transform.DOLocalMove(originalPosition, hoverDuration).SetEase(Ease.OutQuad);
        }

        private void OnClick()
        {
            loadoutTableUI.DisplayWeaponUI(this);
        }

        public void Select()
        {
            isSelected = true;
            weapon.transform.DOKill();
            weapon.transform.DOMove(loadoutTableUI.SelectedWeaponTarget.position, hoverDuration).SetEase(Ease.OutQuad);
        }

        public void Deselect()
        {
            isSelected = false;
            weapon.transform.DOKill();
            weapon.transform.DOLocalMove(originalPosition, hoverDuration).SetEase(Ease.OutQuad);
        }

        private bool IsInteractable()
        {
            return !isSelected && loadoutTableUI.CurrentWeaponHolderUI == null;
        }
    }
}