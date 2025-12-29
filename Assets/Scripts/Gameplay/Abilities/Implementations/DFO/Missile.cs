using System;
using Ami.BroAudio;
using KBCore.Refs;
using OverBang.GameName.Core;
using UnityEngine;
using UnityUtils;

namespace OverBang.GameName.Gameplay
{
    public class Missile : MonoBehaviour
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField] private ParticleSystem sparks;
        [SerializeField] private ParticleSystem smoke;
        
        public event Action<Missile> OnDetonate;
        
        private DfoData data;
        private RaycastHit[] results;
        private ParticleSystem preview;

        private void OnValidate() => this.ValidateRefs();

        public void Initialize(DfoData dfoData)
        {
            data = dfoData;
            results = new RaycastHit[10];
            
            rb.AddForce(Vector3.down * (data.MissileSpeed * Time.deltaTime), ForceMode.Impulse);

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1000f,
                    GameMetrics.Global.HittableLayers, QueryTriggerInteraction.Ignore))
            {
                preview = Instantiate(data.PreviewPrefab, hit.point.Add(y: 0.25f), Quaternion.identity);
            }
        }

        public void OnTick(float deltaTime)
        {
            
        }

        private void OnCollisionEnter(Collision other)
        {
            Invoke(nameof(Detonate), data.DetonationTime);
        }

        private void Detonate()
        {
            //VFX
            ParticleSystem explosion = Instantiate(data.ExplosionPrefab, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration);
            
            ParticleSystem impact = Instantiate(data.ImpactPrefab, transform.position, Quaternion.identity);
            impact.Play();
            Destroy(impact.gameObject, impact.main.duration);
            
            //Audio
            BroAudio.Play(data.DetonationSound, transform.position);
            
            //Damage
            int hitCount = Physics.SphereCastNonAlloc(transform.position, data.DetonationRadius, Vector3.down, results,
                data.DetonationRadius, GameMetrics.Global.HittableLayers, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = results[i];
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(data.MissileDamage);
                }
            }

            //Cleanup
            sparks.transform.parent = null;
            sparks.Stop();
            Destroy(sparks.gameObject, sparks.main.duration);
            
            smoke.transform.parent = null;
            smoke.Stop();
            Destroy(smoke.gameObject, smoke.main.duration);
            
            if (preview != null)
                Destroy(preview.gameObject);
            
            OnDetonate?.Invoke(this);
            Destroy(gameObject, 0.1f);
        }
    }
}