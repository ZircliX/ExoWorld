using Ami.BroAudio;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    public class CdQueued
    {
        public bool IsPlaying => AudioPlayer is { IsPlaying: true };
        public bool WasFired => AudioPlayer is not null;
        public bool IsFinished => WasFired && !IsPlaying;
        public IAudioPlayer AudioPlayer { get; private set; }
        
        public readonly ContextualDialogue dialogue;
        public readonly CDContext context;
        
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
            
            if (lifetime >= dialogue.lifetime)
            {
                outDated = true;
            }
        }

        public void Fire()
        {
            AudioPlayer = BroAudio.Play(dialogue.soundID, context.sourceTransform);
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