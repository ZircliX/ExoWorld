using DG.Tweening;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.HUB
{
    public class PlanetRotator : MonoBehaviour
    {
        [Header("Objects to Rotate")] 
        [SerializeField] private Transform planet;
        [SerializeField] private Transform littleplanet;
        [SerializeField] private Transform sun;
        [SerializeField] private Transform littlesun;
        [SerializeField] private Transform moon;
        [SerializeField] private Transform littlemoon;
        [SerializeField] private Transform panela;
        [SerializeField] private Transform panelb;
        [SerializeField] private Transform panelc;
        [SerializeField] private Transform paneld;

        [Header("Values")] 
        [SerializeField] private Vector3 planetrotation;
        [SerializeField] private Vector3 littleplanetrotation;
        [SerializeField] private Vector3 sunrotation;
        [SerializeField] private Vector3 littlesunrotation;
        [SerializeField] private Vector3 moonrotation;
        [SerializeField] private Vector3 littlemoonrotation;
        [SerializeField] private Vector3 panelarotation;
        [SerializeField] private Vector3 panelbrotation;
        [SerializeField] private Vector3 panelcrotation;
        [SerializeField] private Vector3 paneldrotation;
        [SerializeField] private float planetRotationSpeed = 20f;
        [SerializeField] private float littleplanetRotationSpeed = 20f;
        [SerializeField] private float sunRotationSpeed = 20f;
        [SerializeField] private float littlesunRotationSpeed = 20f;
        [SerializeField] private float moonRotationSpeed = 20f;
        [SerializeField] private float littlemoonRotationSpeed = 20f;
        [SerializeField] private float panelaRotationSpeed = 20f;
        [SerializeField] private float panelbRotationSpeed = 20f;
        [SerializeField] private float panelcRotationSpeed = 20f;
        [SerializeField] private float paneldRotationSpeed = 20f;
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
            
            littleplanet
                .DOLocalRotate(
                    littleplanetrotation.normalized * 360f,
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
            
            littlesun
                .DOLocalRotate(
                    littlesunrotation.normalized * 360f,
                    duration,
                    RotateMode.LocalAxisAdd
                )
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
            
            moon
                .DOLocalRotate(
                    moonrotation.normalized * 360f,
                    duration,
                    RotateMode.LocalAxisAdd
                )
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
            
            littlemoon
                .DOLocalRotate(
                    littlemoonrotation.normalized * 360f,
                    duration,
                    RotateMode.LocalAxisAdd
                )
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
            
            panela
                .DOLocalRotate(
                    panelarotation.normalized * 360f,
                    duration,
                    RotateMode.LocalAxisAdd
                )
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
            
            panelb
                .DOLocalRotate(
                    panelbrotation.normalized * 360f,
                    duration,
                    RotateMode.LocalAxisAdd
                )
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
            
            panelc
                .DOLocalRotate(
                    panelcrotation.normalized * 360f,
                    duration,
                    RotateMode.LocalAxisAdd
                )
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
            
            paneld
                .DOLocalRotate(
                    paneldrotation.normalized * 360f,
                    duration,
                    RotateMode.LocalAxisAdd
                )
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
    }
}