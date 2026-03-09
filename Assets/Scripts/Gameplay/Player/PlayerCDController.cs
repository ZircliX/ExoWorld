using System.Collections.Generic;
using OverBang.ExoWorld.Core.Audios.ContextualDialogues;
using Sirenix.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Player
{
    public class PlayerCDController : NetworkBehaviour
    {
        private static readonly Dictionary<ulong, PlayerCDController> playerCDControllers = new();

        private void OnEnable()
        {
            playerCDControllers.Add(OwnerClientId, this);
            ContextualDialogueManager.Controller.RegisterPlayer(OwnerClientId);
        }

        private void OnDisable()
        {
            playerCDControllers.Remove(OwnerClientId);
            ContextualDialogueManager.Controller.UnregisterPlayer(OwnerClientId);
        }

        public CDContext GetContext()
        {
            return new CDContext()
            {
                playerId = OwnerClientId,
                sourceTransform =  transform,
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
                    FireDialogueRpc(dialogue.ID);
                }
                else
                {
                    CDContext context = GetContext();
                    ContextualDialogueManager.FireEvent(dialogue.ID, context);
                }
            }
        }
        
        [Rpc(SendTo.Everyone)]
        private void FireDialogueRpc(string id)
        {
            CDContext context = GetContext();
            ContextualDialogueManager.FireEvent(id, context);
        }
        
    }
}