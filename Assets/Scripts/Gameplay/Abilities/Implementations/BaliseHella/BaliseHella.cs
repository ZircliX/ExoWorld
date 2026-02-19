using KBCore.Refs;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class BaliseHella : NetworkBaseDetector
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private NetworkObject networkObject;
        [SerializeField] private Transform bottom;
        
        private ParticleSystem healingCircle;

        public void Initialize(BaliseHellaData data, Vector3 direction, float radius, float duration)
        {
            // Setup detection area
            DetectionArea.GetCollider<SphereCollider>().radius = radius;
            DetectionArea.SetAllowedTags("Player", "LocalPlayer");
            
            rb.AddForce(Vector3.up * 0.5f + direction * data.ThrowForce * Time.deltaTime, ForceMode.Impulse);

            // VFX
            BaliseVfxInitializer vfx = Instantiate(data.VfxInitializer, bottom.position, Quaternion.identity, transform);
            vfx.InitializeVfx(duration, radius);
            healingCircle = vfx.HealingCircle;
            
            healingCircle.Play();
        }

        private void Update()
        {
            if (rb.linearVelocity.sqrMagnitude < 1f)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        public void Stop()
        {
            if (healingCircle != null)
                healingCircle.Stop();
            
            networkObject.Despawn(true);
        }
    }
}