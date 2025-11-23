using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class PumpStart : MonoBehaviour
    {
        [SerializeField] private Pump pump;
        [SerializeField] private GameObject startUI;
        private bool playerInArea;

        private void Start()
        {
            startUI.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !pump.IsStarted.Value && !pump.IsCompleted)
            {
                playerInArea = true;
                startUI.SetActive(true);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInArea = false;
                startUI.SetActive(false);
            }
        }

        private void Update()
        {
            if (Keyboard.current.fKey.wasPressedThisFrame && playerInArea)
            {
                startUI.SetActive(false);
                pump.CallStartRepair();
            }
        }
    }
}