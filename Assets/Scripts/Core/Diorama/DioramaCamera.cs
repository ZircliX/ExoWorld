using System;
using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace OverBang.GameName.Core
{
    public class DioramaCamera : MonoBehaviour
    {
        [field: SerializeField, Self] public CinemachineCamera Cam;
        [field: SerializeField, Self] public Animator animator;
        [SerializeField] private PlayableDirector director;

        private void OnValidate() => this.ValidateRefs();
        
        
        
        private void Update()
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                animator.SetTrigger("isScreaming");
            }

            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                director.Play();

            }
        }
    }
}