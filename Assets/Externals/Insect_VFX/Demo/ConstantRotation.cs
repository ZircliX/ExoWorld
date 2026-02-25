using UnityEngine;

namespace Insect_VFX_DEMO
{
    public class ConstantRotation : MonoBehaviour
    {
        [SerializeField] float speed;

        void Update()
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
        }
    }
}