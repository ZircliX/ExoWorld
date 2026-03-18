using System.Collections.Generic;
using OverBang.ExoWorld.Core.Audios.ContextualDialogues;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.GameMode.Players;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OverBang.ExoWorld.Gameplay.Player
{
    public class PlayerCDController : NetworkBehaviour
    {
        public static PlayerCDController Instance { get; private set; }
        [field : SerializeField] public NetworkObject AudioSourceAnchor { get; private set; }
        public void Awake()
        {
            if (Instance ==null)
            {
                Instance = this;
            }
        }

        private static readonly Dictionary<ulong, PlayerCDController> playerCDControllers = new();

        public override void OnNetworkSpawn()
        {
            playerCDControllers.Add(OwnerClientId, this);
            Debug.Log($"registering player to contextualDialogueManager with id : {OwnerClientId} ");
            ContextualDialogueManager.Controller.RegisterPlayer(OwnerClientId);
        }

        public override void OnNetworkDespawn()
        {
            playerCDControllers.Remove(OwnerClientId);
            ContextualDialogueManager.Controller.UnregisterPlayer(OwnerClientId);
        }

        private CDContext GetContext()
        {
            LocalGamePlayer player = GamePlayerManager.Instance.GetLocalPlayer();
            return new CDContext()
            {
                characterDataId = player.CharacterData.ID,
                playerId = OwnerClientId,
                networkObject = new NetworkObjectReference(AudioSourceAnchor),
                sourceType = CDContext.SourceType.FollowSpatialized
            };
        }
        
        
        public void FireDialogue(string id)
        {
            if (!ContextualDialogueManager.TryGetDialogueData(id, out ContextualDialogueData dialogue))
                return;
            
            FireDialogue(dialogue);
        }

        public void FireDialogue(ContextualDialogueData dialogue)
        {
            float rdn = Random.value;
            
            if (rdn <= dialogue.Probability)
            {
                if (dialogue.CanBeHeardByEveryone)
                {
                    FireDialogueRpc(dialogue.ID, GetContext());
                }
                else
                {
                    CDContext context = GetContext();
                    ContextualDialogueManager.FireEvent(dialogue.ID, context);
                }
            }
        }
        
        [Rpc(SendTo.Everyone)]
        private void FireDialogueRpc(string id, CDContext context)
        {
            
            ContextualDialogueManager.FireEvent(id, context);
        }
        
    }
}