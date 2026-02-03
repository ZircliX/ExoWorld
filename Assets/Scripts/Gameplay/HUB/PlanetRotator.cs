using DG.Tweening;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.HUB
{
    public class PlanetRotator : MonoBehaviour
    {
        [Header("Objects to Rotate")] 
        [SerializeField] private Transform planet;
        [SerializeField] private Transform sun;

        [Header("Values")] 
        [SerializeField] private Vector3 planetrotation;
        [SerializeField] private Vector3 sunrotation;
        [SerializeField] private float planetRotationSpeed = 20f;
        [SerializeField] private float sunRotationSpeed = 20f;
        [SerializeField] private float duration = 20f;
        
        private void Start()
        {
            RotateObject();
        }

        private void RotateObject()
        {
            planet
                .DOLocalRotate(
                    planetrotation.normalized * 360f,
                    duration,
                    RotateMode.LocalAxisAdd
                )
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
            
            sun
                .DOLocalRotate(
                    sunrotation.normalized * 360f,
                    duration,
                    RotateMode.LocalAxisAdd
                )
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
    }
}