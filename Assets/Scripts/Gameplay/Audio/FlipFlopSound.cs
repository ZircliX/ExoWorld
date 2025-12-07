using Ami.BroAudio;
using Ami.BroAudio.Demo;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Audio
{
    public class FlipFlopSound : InteractiveComponent
    {
        [SerializeField] private bool flipIsDefault = true;
        [SerializeField] private SoundID flipSound;
        [SerializeField] private SoundID flopSound;
        
        [Space]
        [SerializeField] private float exitFadeTime = 1f;
        [SerializeField] private float enterFadeTime = 0.5f;

        private SoundID currentSound;

        protected override void Awake()
        {
            base.Awake();

            currentSound = flipIsDefault ? flipSound : flopSound;
            BroAudio.Play(currentSound);
        }

        protected override void OnInZoneChanged(InteractiveZone zone, bool isInZone)
        {
            base.OnInZoneChanged(zone, isInZone);
            
            if (isInZone)
            {
                SoundID nextSound = currentSound.Equals(flipSound) ? flopSound : flipSound;
                BroAudio.Stop(currentSound, exitFadeTime);
                BroAudio.Play(nextSound, enterFadeTime);
                
                currentSound = nextSound;
            }
        }
    }
}