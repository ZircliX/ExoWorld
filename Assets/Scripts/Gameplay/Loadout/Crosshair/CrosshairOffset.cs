using DG.Tweening;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.Crosshair
{
    public class CrosshairOffset : MonoBehaviour
    {
        [SerializeField] private RectTransform rt;
        [SerializeField] private WeaponController wc;
        
        private Weapon weapon;
        private Tween currentTween;

        private void OnEnable()
        {
            wc.OnWeaponChanged += OnWeaponChanged;
        }

        private void OnDisable()
        {
            wc.OnWeaponChanged -= OnWeaponChanged;
        }

        private void OnWeaponChanged(Weapon previous, Weapon current)
        {
            if (previous != null)
                previous.OnWeaponFired -= OnWeaponFired;

            current.OnWeaponFired += OnWeaponFired;
            weapon = current;
            
            CrosshairOffsetData crosshairData = weapon.WeaponData.CrosshairOffsetData;
            FireOffset(crosshairData.MinOffset,
                crosshairData.MinOffsetTime,
                crosshairData.MinEase);
        }

        private void OnWeaponFired()
        {
            CrosshairOffsetData crosshairData = weapon.WeaponData.CrosshairOffsetData;
            
            FireOffset(crosshairData.MaxOffset,
                crosshairData.MaxOffsetTime,
                crosshairData.MaxEase)
                
                .OnComplete(() => FireOffset(crosshairData.MinOffset,
                    crosshairData.MinOffsetTime,
                    crosshairData.MinEase));
        }

        private Tween FireOffset(Vector2 offset, float duration, Ease ease)
        {
            currentTween?.Kill();
            
            currentTween = DOTween.To(
                () => rt.sizeDelta,
                value => rt.sizeDelta = value,
                offset,
                duration
            ).SetEase(ease);
    
            return currentTween;
        }
    }
}