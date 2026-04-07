using System.Collections;
using Helteix.ChanneledProperties.Priorities;
using KBCore.Refs;
using OverBang.ExoWorld.Gameplay.Cameras.Composits;
using Unity.Cinemachine;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] private CinemachineCamera cam;
        [SerializeField, Self] private CinemachineCameraOffset camFollow;
        [SerializeField, Self] private CinemachineRecomposer camRecomposer;
        
        [Header("Camera Properties")]
        public Priority<CameraShakeComposite> CameraShakeProperty { get; private set; }
        public Priority<CameraEffectComposite> CameraEffectProperty { get; private set; }

        private CameraShakeComposite currentShakeComposite;
        private CameraShakeComposite targetShakeComposite;
        private CameraShakeComposite baseComposite; // pan/dutch source
        [SerializeField] private float headBumpBlendDuration = 0.2f;
        [SerializeField] private float headBumpReactiveSpeed = 8f;
        private float headBumpTimer;
        private float blendTimer;
        private float currentTilt;
        private bool isBlending;
        
        private CameraEffectComposite currentEffectComposite;
        private Coroutine effectCoroutine;
        
        private void OnValidate() => this.ValidateRefs();

        private void Awake()
        {
            CameraShakeProperty = new Priority<CameraShakeComposite>();
            CameraShakeProperty.AddOnValueChangeCallback(ApplyCameraShake, true);

            CameraEffectProperty = new Priority<CameraEffectComposite>(CameraEffectComposite.Default);
            CameraEffectProperty.AddOnValueChangeCallback(ApplyCameraEffect, true);
        }
        
        private void UpdateShake()
        {
            if (isBlending)
            {
                blendTimer += Time.deltaTime;
                float t = Mathf.Clamp01(blendTimer / headBumpBlendDuration);

                if (!targetShakeComposite.onlyTilt)
                {
                    currentShakeComposite.panAmplitude = Mathf.Lerp(currentShakeComposite.panAmplitude, targetShakeComposite.panAmplitude, t);
                    currentShakeComposite.tiltAmplitude = Mathf.Lerp(currentShakeComposite.tiltAmplitude, targetShakeComposite.tiltAmplitude, t);
                    currentShakeComposite.dutchAmplitude = Mathf.Lerp(currentShakeComposite.dutchAmplitude, targetShakeComposite.dutchAmplitude, t);
                    currentShakeComposite.frequency = targetShakeComposite.frequency;
                }

                if (blendTimer >= headBumpBlendDuration)
                    isBlending = false;
            }

            if (currentShakeComposite.frequency <= 0f)
            {
                camRecomposer.Pan = Mathf.Lerp(camRecomposer.Pan, 0f, Time.deltaTime * headBumpReactiveSpeed);
                camRecomposer.Dutch = Mathf.Lerp(camRecomposer.Dutch, 0f, Time.deltaTime * headBumpReactiveSpeed);

                if (targetShakeComposite.onlyTilt)
                    currentTilt = Mathf.Lerp(currentTilt, targetShakeComposite.tiltAmplitude, Time.deltaTime * headBumpReactiveSpeed);
                else
                    currentTilt = Mathf.Lerp(currentTilt, 0f, Time.deltaTime * headBumpReactiveSpeed);

                camRecomposer.Tilt = currentTilt;
                return;
            }

            float cycleDuration = 1f / currentShakeComposite.frequency;

            if (!targetShakeComposite.onlyTilt)
            {
                headBumpTimer += Time.deltaTime;
                if (headBumpTimer >= cycleDuration)
                    headBumpTimer = 0f;
            }

            float t2 = headBumpTimer / cycleDuration;
            float curve = Mathf.Sin(t2 * Mathf.PI * 2f);

            float panSource = targetShakeComposite.onlyTilt ? baseComposite.panAmplitude : currentShakeComposite.panAmplitude;
            float dutchSource = targetShakeComposite.onlyTilt ? baseComposite.dutchAmplitude : currentShakeComposite.dutchAmplitude;

            camRecomposer.Pan = curve * panSource;
            camRecomposer.Dutch = curve * dutchSource;

            if (targetShakeComposite.onlyTilt)
                currentTilt = Mathf.Lerp(currentTilt, targetShakeComposite.tiltAmplitude, Time.deltaTime * headBumpReactiveSpeed);
            else
                currentTilt = Mathf.Lerp(currentTilt, Mathf.Abs(curve) * currentShakeComposite.tiltAmplitude, Time.deltaTime * headBumpReactiveSpeed);

            camRecomposer.Tilt = currentTilt;
        }
        
        private void ApplyCameraShake(CameraShakeComposite composite)
        {
            if (targetShakeComposite.Equals(composite)) return;
    
            if (!composite.onlyTilt)
            {
                baseComposite = currentShakeComposite;

                if (targetShakeComposite.onlyTilt)
                    currentShakeComposite.tiltAmplitude = camRecomposer.Tilt;

                if (currentShakeComposite.frequency > 0f && composite.frequency > 0f)
                {
                    float currentCycleDuration = 1f / currentShakeComposite.frequency;
                    float currentProgress = headBumpTimer / currentCycleDuration;
                    float newCycleDuration = 1f / composite.frequency;
                    headBumpTimer = currentProgress * newCycleDuration;
                }
            }

            targetShakeComposite = composite;
            blendTimer = 0f;
            isBlending = true;
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

        private Vector3 targetRotation;
        private Vector3 currentRotation;
        private float snap;
        private float returnSpeed;
        
        private void Update()
        {
            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
            currentRotation = Vector3.Slerp(currentRotation, targetRotation, snap * Time.fixedDeltaTime);
            Quaternion recoilRotation = Quaternion.Euler(currentRotation);
            
            transform.localRotation = recoilRotation;
            
            UpdateShake();   
        }

        public void RecoilFire(Vector3 targetRotation, float snap, float returnSpeed)
        {
            this.targetRotation = targetRotation;
            this.snap = snap;
            this.returnSpeed = returnSpeed;
        }
    }
}