using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.Core.Core.Network
{
    public class GameModeHandler : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] networkAdapters;
        [SerializeField] private MonoBehaviour[] soloAdapters;
        
        private void Start()
        {
            GameMode.GameModeType gameMode = GameController.GameMode.CurrentGameMode;
            UpdateAdapters(gameMode);
        }

        private void UpdateAdapters(GameMode.GameModeType gameMode)
        {
            bool isSolo = gameMode == GameMode.GameModeType.Solo;

            for (int index = 0; index < networkAdapters.Length; index++)
            {
                MonoBehaviour adapter = networkAdapters[index];
                adapter.enabled = !isSolo;
            }

            for (int index = 0; index < soloAdapters.Length; index++)
            {
                MonoBehaviour adapter = soloAdapters[index];
                adapter.enabled = isSolo;
            }
        }
    }
}