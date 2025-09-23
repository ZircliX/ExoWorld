using OverBang.GameName.Core;
using OverBang.GameName.Managers;
using OverBang.GameName.Offline;
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
                //offlineGameMode.StateMachine.ChangeState(new GameplayState(offlineGameMode.StateMachine, offlineGameMode, offlineGameMode));
            }
            else
            {
                Destroy(this);
            }
        }
    }
}
