using LTX.ChanneledProperties.Priorities;
using UnityEngine;

namespace DeadLink.Gravity
{
    public abstract class GravityReceiver : MonoBehaviour
    {
        public static Vector3 DefaultGravityDirection = Vector3.down;
        public const float GRAVITY_MULTIPLIER = 9.81f;

        public abstract Vector3 Position { get; }
        public Priority<Vector3> Gravity { get; private set; }

        protected virtual void Awake()
        {
            Gravity = new Priority<Vector3>(Vector3.down);
        }
        
        protected Vector3 GetGravityDirection()
        {
            Vector3 gravityDirection = Gravity.ChannelCount == 0 ? DefaultGravityDirection : Gravity.Value;

            return gravityDirection * GRAVITY_MULTIPLIER;
        }
    }
}