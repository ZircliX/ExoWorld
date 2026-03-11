using System;
using DG.Tweening;
using OverBang.ExoWorld.Gameplay.Loadout;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Manena
{
    public class CrosshairOffset : MonoBehaviour
    {
        [SerializeField] private RectTransform rt;
        [SerializeField] private WeaponController wc;
        private Weapon weapon;

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
            Debug.LogError("Haaaaaaaaaaaaaaa");
        }

        private void OnWeaponFired()
        {
            FireOffset(weapon.WeaponData.CrosshairOffsetData.MaxOffset).OnComplete(() => FireOffset(weapon.WeaponData.CrosshairOffsetData.MinOffset));
            Debug.LogError("Haaaaaaaaaaaaaaa");
        }

        private Tween FireOffset(Vector2 offset)
        {
            Debug.LogError("Haaaaaaaaaaaaaaa");
            return DOTween.To(
                () => rt.sizeDelta,
                value => rt.sizeDelta = value,
                offset,
                weapon.WeaponData.CrosshairOffsetData.OffsetTime
            ).SetEase(weapon.WeaponData.CrosshairOffsetData.EaseCrosshair);
        }
    }
}