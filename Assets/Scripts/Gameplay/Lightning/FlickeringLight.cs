using KBCore.Refs;
using OverBang.ExoWorld.Core.Metrics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OverBang.ExoWorld.Gameplay.Lightning
{
    [ExecuteInEditMode]
    public class FlickeringLight : MonoBehaviour
    {
        [SerializeField, Self] private Light targetLight;

        [SerializeField] private float defaultIntensity;
        
        [Space]
        [SerializeField] private float minTime;
        [SerializeField] private float maxTime;

        [Space]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip audioClip;

        private float timer;
        private bool isLit;
    
        private void OnValidate()
        {
            this.ValidateRefs();
            
            if (timer == 0)
                timer = Random.Range(minTime, maxTime);

            isLit = true;
            targetLight.intensity = defaultIntensity;
        }

        private void OnEnable()
        {
            if (Application.isPlaying)
                this.Register();
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
                this.Unregister();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (GameMetrics.Global.LightFlickerInEditMode)
                FlickerLight();
        }
#endif

        public void FlickerLight()
        {
            if (timer > 0)
                timer -= Time.deltaTime;

            if (timer <= 0)
            {
                isLit = !isLit;
                targetLight.intensity = isLit ? defaultIntensity : 0f;
                timer = Random.Range(minTime, maxTime);
                
                if (audioClip != null)
                    audioSource.PlayOneShot(audioClip);
            }
        }
    }
}
