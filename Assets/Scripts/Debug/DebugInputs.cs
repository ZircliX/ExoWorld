using OverBang.GameName.Core;
using OverBang.GameName.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace OverBang.GameName.Debug
{
    public class DebugInputs : MonoBehaviour
    {
        [SerializeField] private PlayerProfile profile;
        private static DebugInputs instance;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (instance == null)
            {
                Instantiate(GameController.Metrics.DebugInputs);
            }
        }
        
        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Update()
        {
            if (Keyboard.current.numpad1Key.wasPressedThisFrame)
            {
                SceneManager.LoadScene("MainMenu");
            }

            if (Keyboard.current.numpad2Key.wasPressedThisFrame)
            {
                SceneManager.LoadScene("Hub");
            }

            if (Keyboard.current.numpad3Key.wasPressedThisFrame)
            {
                SceneManager.LoadScene("Map");
            }
        }
    }
}