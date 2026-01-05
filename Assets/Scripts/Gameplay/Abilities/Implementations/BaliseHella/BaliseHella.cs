using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class BaliseHella : MonoBehaviour
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Child] private DetectionArea detectionArea;
        [SerializeField] private Transform bottom;

        private List<IDamageable> damageables;
        private ParticleSystem healingCircle;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        private void OnEnable()
        {
            detectionArea.OnEnter += OnEnter;
            detectionArea.OnExit += OnExit;
        }

        private void OnDisable()
        {
            detectionArea.OnEnter -= OnEnter;
            detectionArea.OnExit -= OnExit;
        }

        public void Initialize(BaliseHellaData data, Vector3 direction)
        {
            detectionArea.GetCollider<SphereCollider>().radius = data.Radius;
            detectionArea.SetAllowedTags("Player", "LocalPlayer");
            detectionArea.SetRequireInterface<IDamageable>();
            
            damageables = new List<IDamageable>(4);
            rb.AddForce(Vector3.up * 0.5f + direction * data.ThrowForce * Time.deltaTime, ForceMode.Impulse);

            healingCircle = Instantiate(data.HealingCircle, bottom.position, Quaternion.identity, transform);
            ParticleSystem.MainModule main = healingCircle.main;
            main.duration = data.Duration;
            main.startSize = data.Radius;
            
            healingCircle.Play();
        }
        
        public void Stop()
        {
            foreach (IDamageable damageable in damageables)
            {
                damageable.IsInvincible = false;
            }
            
            if (healingCircle == null)
                return;
            
            healingCircle.Stop();
            Destroy(gameObject);
        }

        private void OnEnter(Collider other, object target)
        {
            IDamageable damageable = (IDamageable) target;
            
            damageables.Add(damageable);
            damageable.IsInvincible = true;
        }

        private void OnExit(Collider other, object target)
        {
            IDamageable damageable = (IDamageable) target;
            
            damageables.Remove(damageable);
            damageable.IsInvincible = false;
        }
    }
}