using UnityEngine;

namespace Insect_VFX
{
    public class Entity
    {
        public GameObject objectEmmited;
        public bool dieOnFinishPath;
        public Vector3 targetPosition;
        public Vector3[] pathPoints;
        public float pauseTimer;

        public Entity()
        {
            objectEmmited = null;
            dieOnFinishPath = false;
            targetPosition = Vector3.zero;
            pathPoints = new Vector3[0];
            pauseTimer = 0f;
        }
    }
}
