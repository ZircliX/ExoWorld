using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay.Quests
{
    [RequireComponent(typeof(Collider))]
    public class RepairStart : MonoBehaviour
    {
        [SerializeField] private GameObject repairArea;
        [SerializeField] private GameObject press;
        [SerializeField] private GameObject stay;
        private bool playerInArea;

        private void Start()
        {
            repairArea.SetActive(false);
            press.SetActive(false);
            stay.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !stay.activeInHierarchy)
            {
                playerInArea = true;
                press.SetActive(true);
                Debug.Log("Player entered repair area");
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && !stay.activeInHierarchy)
            {
                playerInArea = false;
                press.SetActive(false);
                Debug.Log("Player exited repair area");
            }
        }

        private void Update()
        {
            if (Keyboard.current.fKey.wasPressedThisFrame && playerInArea)
            {
                repairArea.SetActive(true);
                press.SetActive(false);
                stay.SetActive(true);
            }
        }
    }
}