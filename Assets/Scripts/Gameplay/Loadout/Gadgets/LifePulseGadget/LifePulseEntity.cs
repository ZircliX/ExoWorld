using System.Collections;
using Ami.BroAudio;
using KBCore.Refs;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Gameplay.Abilities;
using OverBang.ExoWorld.Gameplay.Player;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.LifePulseGadget
{
    public class LifePulseEntity : NetworkBehaviour
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField] private Collider collider;
        
        private IExplosionStrategy strategy;

        private LifePulseData data;
        private LifePulse lifePulseGrenade;
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
        }
        
        public void Initialize(LifePulseData data, LifePulse lifePulse)
        {
            this.lifePulseGrenade = lifePulse;
            this.data = data;
            FreezeGrenade(false);
            
            LocalGamePlayer localGamePlayer = GamePlayerManager.Instance.GetLocalPlayer();
            Transform player = localGamePlayer.CurrentPlayerObject.transform.GetChild(1);
            if (player.TryGetComponent(out PlayerEntity playerEntity))
            {
                playerEntity.Heal(data.HealthAmount);
                MeshRenderer meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
                meshRenderer.enabled = false;
                StartCoroutine(OnHealed());
            }
        }
        
        private IEnumerator OnHealed()
        {
            BroAudio.Play(data.SoundID);
            
            if (data.ExplosionEffect != null)
            {
                //ParticleSystem ps = Instantiate(data.ExplosionEffect, transform.position, Quaternion.identity);
                
                //float mainDuration = ps.main.duration;
                //Destroy(ps.gameObject, mainDuration);
                
                //yield return new WaitForSeconds(mainDuration);
                yield return null; //TODO : delete
                End();
            }
        }
        
        private void End()
        {
            lifePulseGrenade.End();
            Destroy(gameObject);
        }
    }
}