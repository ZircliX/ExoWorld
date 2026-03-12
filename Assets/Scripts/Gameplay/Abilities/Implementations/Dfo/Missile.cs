using Ami.BroAudio;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Targeting;
using Unity.Netcode;
using UnityEngine;
using UnityUtils;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class Missile : MonoBehaviour, IDamageSource
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private NetworkObject networkObject;
        [SerializeField, Self] private SoundSource soundSource;
        [SerializeField] private ParticleSystem missileSmoke;

        private MissileManager manager;
        private float lifeTime;
        private DamageData damageData;
        private bool isDetonated;
        
        private MissileData data;
        private RaycastHit[] results;
        private ParticleSystem preview;

        private void OnValidate() => this.ValidateRefs();

        private void Awake()
        {
            results = new RaycastHit[10];
        }

        public void Initialize(MissileData data, MissileManager manager, DamageData damageData)
        {
            this.data = data;
            this.manager = manager;
            this.damageData = damageData;
            
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
            if (isDetonated)
                return;
            
            lifeTime += deltaTime;
            
            if (lifeTime >= data.LifeTime)
            {
                Cleanup();
                isDetonated = true;
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
                    Damage(damageable);
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
        }

        private void Cleanup()
        {
            //Cleanup
            missileSmoke.Stop();
            missileSmoke.gameObject.SetActive(false);
            
            if (preview != null) Destroy(preview.gameObject);
            soundSource.Stop();
            
            manager.RemoveMissile(this);
            networkObject.Despawn();
        }

        public DamageData DamageData => damageData;
        public void Damage(IDamageable damageable)
        {
            RuntimeDamageData runtimeDamageData = DamageData.GetRuntimeDamage();
            
            damageable.TakeDamage(runtimeDamageData);
            data.damagePrefab.Spawn(damageable.DamageTarget.position, runtimeDamageData.finalDamage, damageable.DamageTarget);
        }
    }
}