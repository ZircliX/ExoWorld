using Ami.BroAudio;
using KBCore.Refs;
using OverBang.GameName.Core;
using Unity.Netcode;
using UnityEngine;
using UnityUtils;

namespace OverBang.GameName.Gameplay
{
    public class Missile : MonoBehaviour
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private NetworkObject networkObject;
        [SerializeField, Self] private SoundSource soundSource;
        [SerializeField] private ParticleSystem missileSmoke;

        private MissileManager manager;
        private float lifeTime;
        
        private MissileData data;
        private RaycastHit[] results;
        private ParticleSystem preview;

        private void OnValidate() => this.ValidateRefs();

        private void Awake()
        {
            results = new RaycastHit[10];
        }

        public void Initialize(MissileData data, MissileManager manager)
        {
            this.data = data;
            this.manager = manager;
            
            rb.AddForce(Vector3.down * (data.Speed * Time.deltaTime), ForceMode.Impulse);
            soundSource.Play();
            
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1000f,
                    GameMetrics.Global.HittableLayers, QueryTriggerInteraction.Ignore))
            {
                preview = Instantiate(data.PreviewPrefab, hit.point.Add(y: 0.1f), Quaternion.identity);
            }
        }

        public void OnTick(float deltaTime)
        {
            lifeTime += deltaTime;
            
            if (lifeTime >= data.LifeTime)
            {
                Cleanup();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            Invoke(nameof(Detonate), data.DetonationTime);
        }

        private void Detonate()
        {
            HandleDamage();
            HandleFeedbacks();

            Cleanup();
        }

        private void HandleDamage()
        {
            int hitCount = Physics.SphereCastNonAlloc(transform.position, data.DetonationRadius, Vector3.down, results,
                data.DetonationRadius, GameMetrics.Global.HittableLayers, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = results[i];
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(data.Damage);
                }
            }
        }

        private void HandleFeedbacks()
        {
            //VFX
            ParticleSystem explosion = Instantiate(data.ExplosionPrefab, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration);
            
            ParticleSystem impact = Instantiate(data.ImpactPrefab, transform.position.Add(y: -0.25f), Quaternion.identity);
            impact.Play();
            Destroy(impact.gameObject, impact.main.duration);
            
            //Audio
            BroAudio.Play(data.DetonationSound, transform.position);
            
            Cleanup();
        }

        private void Cleanup()
        {
            //Cleanup
            missileSmoke.transform.parent = null;
            missileSmoke.Stop();
            Destroy(missileSmoke.gameObject, missileSmoke.main.duration);
            
            if (preview != null) Destroy(preview.gameObject);
            soundSource.Stop();
            
            manager.RemoveMissile(this);
            networkObject.Despawn();
        }
    }
}