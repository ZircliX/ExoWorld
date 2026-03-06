namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    public class CdQueued
    {
        public ContextualDialogue dialogue;
        public float lifetime;
        public float maxLifetime;
        private bool outDated;

        public void Initialize(ContextualDialogue dialogue, float maxLifetime)
        {
            this.dialogue = dialogue;
            this.maxLifetime = maxLifetime;
            outDated = false;
            lifetime = 0;
        }
            
        public void Tick(float deltaTime)
        {
            lifetime += deltaTime;

            if (lifetime >= maxLifetime)
            {
                outDated = true;
            }
        }

        public bool IsOutDated()
        {
            return outDated;
        }
            
    }
}