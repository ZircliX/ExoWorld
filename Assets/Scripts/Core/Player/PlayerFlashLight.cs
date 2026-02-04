using UnityEngine;

namespace OverBang.ExoWorld.Core.Player
{
    public class PlayerFlashLight : MonoBehaviour
    {
        [SerializeField] private Light flashLight;
        
        public void SetState(bool state)
        {
            flashLight.enabled = state;
        }
    }
}