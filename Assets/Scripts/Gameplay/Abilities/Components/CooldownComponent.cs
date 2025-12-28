using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class CooldownComponent
    {
        private readonly float cooldownTime;
        private float cooldownRemaining;

        public float Remaining => Mathf.Max(0, cooldownRemaining);
        public bool IsReady => cooldownRemaining <= 0;

        public CooldownComponent(float cooldownTime)
        {
            this.cooldownTime = cooldownTime;
            this.cooldownRemaining = 0;
        }

        public void Tick(float deltaTime)
        {
            if (cooldownRemaining > 0)
                cooldownRemaining -= deltaTime;
        }

        public void Start()
        {
            cooldownRemaining = cooldownTime;
        }
    }
}