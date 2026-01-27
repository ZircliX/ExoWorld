using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Menus
{
    public class ServerVisibilityToggle : MonoBehaviour
    {
        [SerializeField] private Button publicButton;
        [SerializeField] private Button privateButton;
        
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite inactiveSprite;
        
        public event Action<ServerVisibility> OnFilterChanged;
        
        private ServerVisibility currentVisibility;
        
        private void Awake()
        {
            publicButton.onClick.AddListener(() => SetVisibility(ServerVisibility.Public));
            privateButton.onClick.AddListener(() => SetVisibility(ServerVisibility.Private));
            
            SetVisibility(ServerVisibility.Public);
        }

        private void SetVisibility(ServerVisibility visibility)
        {
            if (currentVisibility == visibility)
                return;
            
            currentVisibility = visibility;
            UpdateButtonSprites(visibility);
            OnFilterChanged?.Invoke(visibility);
        }
        
        private void UpdateButtonSprites(ServerVisibility visibility)
        {
            bool isPublic = visibility == ServerVisibility.Public;

            publicButton.image.sprite = isPublic ? activeSprite : inactiveSprite;
            publicButton.interactable = !isPublic;

            privateButton.image.sprite = isPublic ? inactiveSprite : activeSprite;
            privateButton.interactable = isPublic;
        }
    }
}