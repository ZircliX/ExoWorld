using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class BaliseVfxInitializer : MonoBehaviour
    {
        [field: SerializeField] public ParticleSystem HealingCircle { get; private set; }
        [SerializeField] private ParticleSystem[] vfxRadius;
        [SerializeField] private ParticleSystem[] vfxSphereRadius;
        [SerializeField] private ParticleSystem[] vfxDuration;

        public void InitializeVfx(float duration, float radius)
        {
            for (int i = 0; i < vfxRadius.Length; i++)
            {
                ParticleSystem ps = vfxRadius[i];
                SetParticleRadius(ps, radius);
            }
            
            for (int i = 0; i < vfxDuration.Length; i++)
            {
                ParticleSystem ps = vfxDuration[i];
                SetParticleDuration(ps, duration);
            }
            
            ParticleSystem.MainModule module = HealingCircle.main;
            module.startSizeXMultiplier = radius;
            module.startSizeYMultiplier = radius;
            
            module = vfxSphereRadius[0].main;
            module.startSizeXMultiplier = radius * 0.49f;
            module.startSizeZMultiplier = radius * 0.49f;
        }
        
        private void SetParticleRadius(ParticleSystem ps, float radius)
        {
            ParticleSystem.ShapeModule main = ps.shape;
            main.radius = radius;
        }
        
        private void SetParticleDuration(ParticleSystem ps, float duration)
        {
            ParticleSystem.MainModule main = ps.main;
            main.startLifetime = duration;
        }
    }
}