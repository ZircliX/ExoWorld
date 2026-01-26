using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ami.BroAudio.Demo
{
    public class SparkleObject : InteractiveComponent
    {
        [SerializeField] SoundID _sound = default;

        protected override bool IsTriggerOnce => true;

        public override void OnInZoneChanged(InteractiveZone zone, bool isInZone)
        {
            base.OnInZoneChanged(zone, isInZone);
            BroAudio.Play(_sound);
            Destroy(gameObject);
        }
    } 
}
