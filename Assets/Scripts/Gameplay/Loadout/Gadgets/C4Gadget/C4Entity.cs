using System;
using Ami.BroAudio;
using KBCore.Refs;
using OverBang.ExoWorld.Gameplay.Abilities;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.C4Gadget
{
    public class C4Entity : MonoBehaviour
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private TrailRenderer trail;
        [SerializeField] private Collider collider;
        
        private IExplosionStrategy strategy;
        
        private C4Data data;
        private C4 frostBiteGrenade;
        private float time;
        private bool isDetonated;

        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        public void FreezeGrenade(bool value)
        {
            rb.isKinematic = value;
            collider.isTrigger = value;
            trail.enabled = !value;
        }
        
        public void Initialize(C4Data data, Vector3 direction, C4 grenade)
        {
            strategy = new StandardExplosion(data.DamageData);
            this.frostBiteGrenade = grenade;
            this.data = data;
            strategy.OnExploded += OnExploded;
            FreezeGrenade(false);
            rb.AddForce(Vector3.up * 0.5f + direction * data.ThrowForce * Time.deltaTime, ForceMode.Impulse);
        }
        
        private void OnExploded(bool terminated)
        {
            BroAudio.Play(data.SoundID);
            
            if (data.ExplosionEffect != null)
            {
                ParticleSystem ps = Instantiate(data.ExplosionEffect, transform.position, Quaternion.identity);
                Destroy(ps.gameObject, ps.main.duration);
            }
            
            if (terminated)
            {
                strategy.OnExploded -= OnExploded;
                End();
            }
        }
        
        private void End()
        {
            frostBiteGrenade.End();
            Destroy(gameObject);
        }
    }
}