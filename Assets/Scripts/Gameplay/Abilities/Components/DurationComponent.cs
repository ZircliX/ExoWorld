using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public struct DurationComponent
    {
        private readonly float duration;
        private float elapsed;

        public float Remaining => Mathf.Max(0, duration - elapsed);
        public bool IsExpired => elapsed >= duration;

        public DurationComponent(float duration)
        {
            this.duration = duration;
            this.elapsed = 0;
        }

        public void Tick(float deltaTime) => elapsed += deltaTime;
        public void Reset() => elapsed = 0;
    }
}