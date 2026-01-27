using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace OverBang.ExoWorld.Core.Diorama
{
    public class DioramaCamera : MonoBehaviour
    {
        [field : SerializeField] public Animator animator { get; private set; }
        [field : SerializeField] public PlayableDirector director { get; private set; }

        
        
        
        private void Update()
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                //play screaming animation
                animator.SetBool("isScreaming", true);
            }
                //Start rotate camera
            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                director.Play();

            }
        }

        public void StopScreaming()
        {
            animator.SetBool("isScreaming", false);
        }
    }
}