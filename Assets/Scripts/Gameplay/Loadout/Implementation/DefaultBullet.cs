using System.Collections.Generic;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Upgrade;
using OverBang.ExoWorld.Gameplay.Targeting;
using OverBang.ExoWorld.Gameplay.Upgrade;
using OverBang.Pooling;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class DefaultBullet : Bullet, IDamageSource
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private SphereCollider sc;
        public override IPool Pool { get; protected set; }

        private BulletData data;
        private float damageMultiplier;
        
        private Vector3 previousPosition;
        private static readonly Collider[] results = new Collider[8];
        private int currentPenetration;
        private float lifeTime;
        
        private List<GameObject> hitObjects = new List<GameObject>();

        private void OnValidate() => this.ValidateRefs();

        private void Awake()
        {
            hitObjects = new List<GameObject>();
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        
            // Only owner simulates physics
            if (!IsOwner)
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
                sc.enabled = false;
            }
        }

        public override void Fire(Vector3 origin, Vector3 direction, BulletData bulletData, float damageMultiplier)
        {
            if (!IsOwner) 
                return;
            
            transform.position = origin;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            
            previousPosition = transform.position;

            data = bulletData;
            this.damageMultiplier = damageMultiplier;
            
            if (rb == null) return;
            rb.AddForce(direction * data.BulletSpeed, ForceMode.Impulse);
        }

        private void FixedUpdate()
        {
            if (!IsOwner)
                return;
            
            lifeTime += Time.fixedDeltaTime;
            if (lifeTime >= data.BulletLifeTime)
            {
                ReturnBullet();
                return;
            }
            
            Vector3 currentPosition = transform.position;
            LayerMask mask = GameMetrics.Global.HittableLayers;
            
            int hitSize = Physics.OverlapCapsuleNonAlloc(previousPosition, currentPosition, sc.radius, results, mask, QueryTriggerInteraction.Ignore);

            //Debug.DrawRay(previousPosition, direction * distance.magnitude, Color.red, 0.5f);
            //Debug.Log(hitSize);
            
            for (int i = 0; i < hitSize; i++)
            {
                Collider hit = results[i];
                //Debug.Log($"Bullet hit : {hit.collider.gameObject.name} with tag {hit.collider.tag}", hit.collider.gameObject);

                if (!hit.gameObject.CompareTag("LocalPlayer"))
                {
                    if (hit.TryGetComponent(out IDamageable damageable))
                    {
                        //Debug.Log($"Dealing damage to IDamageable {hit.collider.name}", hit.collider.gameObject);
                        Damage(damageable);
                    }
                    
                    /* To push objects when hit - disabled for now
                    if(hit.collider.attachedRigidbody != null)
                    {
                        hit.collider.attachedRigidbody.AddForce(rb.velocity * data.ImpactForce, ForceMode.Impulse);
                    }
                    */
                    
                    currentPenetration++;
                    hitObjects.Add(hit.gameObject);
                }
                
                if (currentPenetration >= data.Penetration)
                {
                    ReturnBullet();
                    break;
                }
            }
            
            previousPosition = currentPosition;
        }

        private void ReturnBullet()
        {
            //Debug.Log("Despawning bullet", gameObject);
            if (IsOwner)
                NetworkObject.Despawn();
            else
                gameObject.SetActive(false);
        }

        public override void OnSpawn(IPool pool)
        {
            if (!IsOwner) return;
            Pool = pool;
            
            currentPenetration = 0;
            lifeTime = 0;
            
            if (rb == null) return;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }

        public override void OnDespawn(IPool pool)
        {
            if (!IsOwner) return;
            if (rb == null) return;
            
            if (!rb.isKinematic)
                rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        public DamageData DamageData => data.Damage;
        public void Damage(IDamageable damageable)
        {
            float damage = data.Damage.baseDamage;
            float bonusDamage = UpgradeManager.Instance.GetRuntimeUpgrade(UpgradeType.Damage);
                    
            RuntimeDamageData finalDamage = new RuntimeDamageData()
            {
                finalDamage = ((damage + bonusDamage) * damageMultiplier) / (currentPenetration + 1),
                weakSpotMultiplier = data.Damage.weakSpotMultiplier,
            };
            
            damageable.TakeDamage(finalDamage);
            data.DamagePrefab.Spawn(damageable.DamageTarget.position, finalDamage.finalDamage, damageable.DamageTarget);
        }
    }
}