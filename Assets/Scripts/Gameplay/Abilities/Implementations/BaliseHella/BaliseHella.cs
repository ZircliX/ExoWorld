using KBCore.Refs;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    public class BaliseHella : BaseDetector
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField] private Transform bottom;
        
        private ParticleSystem healingCircle;

        public void Initialize(BaliseHellaData data, Vector3 direction, float radius)
        {
            // Setup detection area
            DetectionArea.GetCollider<SphereCollider>().radius = radius;
            DetectionArea.SetAllowedTags("Player", "LocalPlayer");
            
            rb.AddForce(Vector3.up * 0.5f + direction * data.ThrowForce * Time.deltaTime, ForceMode.Impulse);

            // VFX
            healingCircle = Instantiate(data.HealingCircle, bottom.position, Quaternion.identity, transform);
            ParticleSystem.MainModule main = healingCircle.main;
            main.duration = data.Duration;
            main.startSize = radius;
            
            healingCircle.Play();
        }
        
        public void Stop()
        {
            if (healingCircle == null)
                return;
            
            healingCircle.Stop();
            Destroy(gameObject);
        }
    }
}