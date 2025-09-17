using OverBang.GameName.Managers;
using OverBang.GameName.Offline;
using OverBang.GameName.Offline.CharacterSelectionSystem;
using OverBang.GameName.Player;
using UnityEngine;

namespace OverBang.GameName.Debug
{
    public class DebugMapScene : MonoBehaviour
    {
        [SerializeField] private int difficulty = 0;
        [SerializeField] private PlayerProfile playerProfile;
        
        private void Awake()
        {
            if (GameController.CurrentGameMode == null)
            {
                OfflineGameMode offlineGameMode = OfflineGameMode.Create(0, difficulty).WithPlayer(playerProfile);
                offlineGameMode.SetGameMode();
            }
            else
            {
                Destroy(this);
            }
        }
    }
}
