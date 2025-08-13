using System;
using System.Collections;
using KBCore.Refs;
using LTX.ChanneledProperties.Priorities;
using LTX.Singletons;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OverBang.GameName.Cameras
{
    public class CameraController : SceneSingleton<CameraController>
    {
        [Header("References")]
        [SerializeField, Self] private CinemachineCamera cam;
        [SerializeField, Self] private CinemachineCameraOffset camFollow;
        [SerializeField, Self] private CinemachineRecomposer camRecomposer;
        
        [Header("Camera Properties")]
        public Priority<CameraShakeComposite> CameraShakeProperty { get; private set; }
        public Priority<CameraEffectComposite> CameraEffectProperty { get; private set; }

        private CameraShakeComposite currentShakeComposite;
        private Coroutine shakeCoroutine;
        private Vector3 originalOffset;
        private CameraEffectComposite currentEffectComposite;
        private Coroutine effectCoroutine;
        
        private void OnValidate() => this.ValidateRefs();

        protected override void OnSceneSingletonAwake()
        {
            base.OnSceneSingletonAwake();
            
            CameraShakeProperty = new Priority<CameraShakeComposite>();
            CameraShakeProperty.AddOnValueChangeCallback(ApplyCameraShake, true);

            CameraEffectProperty = new Priority<CameraEffectComposite>(CameraEffectComposite.Default);
            CameraEffectProperty.AddOnValueChangeCallback(ApplyCameraEffect, true);
            
            originalOffset = camFollow.Offset;
        }
        
        private IEnumerator IShakeRoutine()
        {
            float timer = 0f;
            float shakeInterval = 1f / Mathf.Max(currentShakeComposite.Frequency, 0.01f);
            float shakeTimer = 0f;

            Vector3 targetOffset = Vector3.zero;
            Vector3 currentOffset = Vector3.zero;

            while (timer < currentShakeComposite.Duration)
            {
                timer += Time.deltaTime;
                shakeTimer += Time.deltaTime;

                //Debug.Log($"shake timer: {shakeTimer}, shakeInterval: {shakeInterval}");
                if (shakeTimer >= shakeInterval)
                {
                    shakeTimer = 0f;
                    targetOffset = Random.insideUnitSphere * currentShakeComposite.Amplitude;
                }

                currentOffset = Vector3.Lerp(currentOffset, targetOffset, 0.5f);
                camFollow.Offset = originalOffset + currentOffset;
                
                yield return null;
            }

            camFollow.Offset = originalOffset;
        }
        
        private void ApplyCameraShake(CameraShakeComposite composite)
        {
            if (!camFollow.isActiveAndEnabled) return;
            
            currentShakeComposite = composite;

            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
            }

            shakeCoroutine = StartCoroutine(IShakeRoutine());
            //Debug.Log($"Started camera shake with composite: {composite}");
        }

        private IEnumerator IEffectCoroutine()
        {
            float timer = 0f;
            float duration = CameraEffectProperty.Value.Speed;

            float startDutch = camRecomposer.Dutch;
            float targetDutch = CameraEffectProperty.Value.Dutch;

            float startZoom = camRecomposer.ZoomScale;
            float targetZoom = CameraEffectProperty.Value.FovScale;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);

                // Smooth interpolation
                camRecomposer.Dutch = Mathf.Lerp(startDutch, targetDutch, t);
                camRecomposer.ZoomScale = Mathf.Lerp(startZoom, targetZoom, t);

                yield return null;
            }

            // Ensure final values are perfectly set
            camRecomposer.Dutch = targetDutch;
            camRecomposer.ZoomScale = targetZoom;
        }

        private void ApplyCameraEffect(CameraEffectComposite composite)
        {
            if (!camRecomposer.isActiveAndEnabled) return;
            
            currentEffectComposite = composite;
            
            if (effectCoroutine != null)
            {
                StopCoroutine(effectCoroutine);
            }
            
            effectCoroutine = StartCoroutine(IEffectCoroutine());
        }

        private Vector3 _targetRotation;
        private Vector3 currentRotation;
        private float _snap;
        private float _returnSpeed;
        
        private void Update()
        {
            _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, _returnSpeed * Time.deltaTime);
            currentRotation = Vector3.Slerp(currentRotation, _targetRotation, _snap * Time.fixedDeltaTime);
            Quaternion recoilRotation = Quaternion.Euler(currentRotation);
            
            transform.localRotation = recoilRotation;
        }

        public void RecoilFire(Vector3 targetRotation, float snap, float returnSpeed)
        {
            _targetRotation = targetRotation;
            _snap = snap;
            _returnSpeed = returnSpeed;
        }
    }
}