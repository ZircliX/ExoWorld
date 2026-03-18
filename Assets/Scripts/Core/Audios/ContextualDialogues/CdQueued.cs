using Ami.BroAudio;
using OverBang.ExoWorld.Core.Player;
using Unity.Netcode;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    public class CdQueued
    {
        public bool IsPlaying => AudioPlayer is { IsPlaying: true };
        public bool WasFired => AudioPlayer is not null;
        public bool IsFinished => WasFired && !IsPlaying;
        public IAudioPlayer AudioPlayer { get; private set; }
        
        public readonly ContextualDialogue dialogue;
        public CDContext context;
        
        private float lifetime;
        private bool outDated;
        
        public CdQueued(ContextualDialogue dialogue, CDContext context)
        {
            this.dialogue = dialogue;
            this.context = context;
            outDated = false;
            lifetime = 0;
            AudioPlayer = null;
        }

        public void Tick(float deltaTime)
        {
            if (outDated || WasFired) return;
            lifetime += deltaTime;
            
            if (lifetime >= dialogue.voiceLifetime)
            {
                outDated = true;
            }
        }

        public void Fire()
        {
            if(context.networkObject.TryGet(out NetworkObject no))
            {
                if (!no.TryGetComponent(out PlayerReferences playerLinks)) return;
                AudioPlayer = BroAudio.Play(dialogue.soundID, playerLinks.PlayerTransform);
            }           
        }

        public void Kill()
        {
            AudioPlayer?.Stop();
            AudioPlayer = null;
        }

        public bool IsOutDated()
        {
            return outDated;
        }
            
    }
}