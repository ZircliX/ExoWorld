using UnityEngine;
using TMPro;

namespace OverBang.GameName.HUB
{
    public class PlayerCard : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text PlayerName { get; private set; }
        [field: SerializeField] public TMP_Text PlayerStatus { get; private set; }
        
        public PlayerCard SetPlayerName(string newName)
        {
            PlayerName.text = newName;
            return this;
        }

        public PlayerCard SetPlayerStatus(string newStatus)
        {
            PlayerStatus.text = newStatus;
            return this;
        }
    }
}
