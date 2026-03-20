using System.Collections.Generic;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Components;
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
        private static readonly RaycastHit[] sphereResults = new RaycastHit[8];

        private class HitDistanceComparer : IComparer<RaycastHit>
        {
            public static readonly HitDistanceComparer Instance = new();
            public int Compare(RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance);
        }
        
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private SphereCollider sc;
        public override IPool Pool { get; protected set; }
        private Camera cam;

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
            cam = Camera.main;
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

        public override void Fire(Vector3 origin, Vector3 direction, BulletData bulletData, float damageMultiplier, bool shouldCenter)
        {
            if (!IsOwner) return;

            Vector3 correctedDirection = default;
            if (shouldCenter)
            {
                Ray cameraRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of cam
                Vector3 targetPoint = Physics.Raycast(cameraRay, out RaycastHit hit, 500f, GameMetrics.Global.HittableLayers)
                    ? hit.point
                    : cameraRay.origin + cameraRay.direction * 500f;

                correctedDirection = (targetPoint - origin).normalized;
            }
            else
            {
                correctedDirection = direction;
            }

            transform.position = origin;
            transform.rotation = Quaternion.LookRotation(correctedDirection, Vector3.up);

            previousPosition = origin;
            data = bulletData;
            this.damageMultiplier = damageMultiplier;
            
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            lifeTime += Time.fixedDeltaTime;
            if (lifeTime >= data.BulletLifeTime)
            {
                ReturnBullet();
                return;
            }

            float stepDistance = data.BulletSpeed * Time.fixedDeltaTime;
            Vector3 direction = transform.forward;
            LayerMask mask = GameMetrics.Global.HittableLayers;

            // Single reliable SphereCast along the full step
            int hitCount = Physics.SphereCastNonAlloc(
                previousPosition,
                sc.radius,
                direction,
                sphereResults,
                stepDistance,
                mask,
                QueryTriggerInteraction.Ignore
            );

            // Sort by distance so we hit in order
            System.Array.Sort(sphereResults, 0, hitCount, HitDistanceComparer.Instance);

            bool returned = false;
            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = sphereResults[i];
                if (hit.collider == null) continue;
                if (hit.collider.gameObject.CompareTag("LocalPlayer")) continue;
                if (hitObjects.Contains(hit.collider.gameObject)) continue;

                if (hit.collider.TryGetComponent(out IDamageable damageable))
                    Damage(damageable);

                // Spawn decal aligned to normal
                Vector3 decalPoint = hit.point == Vector3.zero ? hit.collider.ClosestPoint(previousPosition) : hit.point;
                Quaternion decalRot = hit.normal != Vector3.zero ? Quaternion.LookRotation(hit.normal) : Quaternion.identity;
                ParticleSystemReference system = Instantiate(data.HitDecalPrefab, decalPoint, decalRot);
                system.Play();
                Destroy(system.gameObject, 10f);

                currentPenetration++;
                hitObjects.Add(hit.collider.gameObject);

                if (currentPenetration >= data.Penetration)
                {
                    ReturnBullet();
                    returned = true;
                    break;
                }
            }

            if (!returned)
            {
                // Move bullet manually
                transform.position = previousPosition + direction * stepDistance;
                previousPosition = transform.position;
            }
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
            hitObjects.Clear();

            rb.isKinematic = true;
        }

        public override void OnDespawn(IPool pool)
        {
            if (!IsOwner) return;
            if (rb == null) return;
            
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
                damageType = DamageType.Projectile,
                itemData = data.ItemData
            };
            
            damageable.TakeDamage(finalDamage);
            data.DamagePrefab.Spawn(damageable.DamageTarget.position, finalDamage.finalDamage, damageable.DamageTarget);
        }
    }
}