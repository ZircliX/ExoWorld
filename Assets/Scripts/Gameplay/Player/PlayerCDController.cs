using OverBang.ExoWorld.Core.Audios.ContextualDialogues;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Gameplay.Phase;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OverBang.ExoWorld.Gameplay.Player
{
    public class PlayerCDController : NetworkBehaviour, IPhaseListener<HubPhase>
    {
        public static PlayerCDController Instance { get; private set; }
        [field : SerializeField] public NetworkObject PlayerTransform { get; private set; }
        public void Awake()
        {
            if (Instance ==null)
            {
                Instance = this;
            }
        }

        public override void OnNetworkSpawn()
        {
            Debug.Log($"registering player to contextualDialogueManager with id : {OwnerClientId} ");
            ulong id = OwnerClientId;
            RegisterPlayerRpc(id);
        }

        [Rpc(SendTo.Everyone)]
        private void RegisterPlayerRpc(ulong id)
        {
            Debug.Log(id);
            Debug.Log(ContextualDialogueManager.Controller);
            ContextualDialogueManager.Controller.RegisterPlayer(id);
        }

        public override void OnNetworkDespawn()
        {
            ContextualDialogueManager.Controller.UnregisterPlayer(OwnerClientId);
            ulong id = OwnerClientId;
            UnregisterPlayerRpc(id);
        }

        [Rpc(SendTo.Everyone)]
        private void UnregisterPlayerRpc(ulong id)
        {
            ContextualDialogueManager.Controller.UnregisterPlayer(id);
        }

        private CDContext GetContext()
        {
            LocalGamePlayer player = GamePlayerManager.Instance.GetLocalPlayer();
            return new CDContext()
            {
                characterDataId = player.CharacterData.ID,
                playerId = OwnerClientId,
                networkObject = new NetworkObjectReference(PlayerTransform),
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
            if (dialogue == null) return;

            float rdn = Random.value;
            
            if (rdn <= dialogue.Probability)
            {
                //Debug.Log("Fire Audio, everyone : " + dialogue.CanBeHeardByEveryone);
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

        public void OnBegin(HubPhase phase)
        {
            phase.OnCharacterSelected += OnCharacterSelected;
        }

        private void OnCharacterSelected(LocalGamePlayer arg1, bool arg2)
        {
            RegisterPlayerRpc(OwnerClientId);
        }

        public void OnEnd(HubPhase phase)
        {
            phase.OnCharacterSelected -= OnCharacterSelected;
        }
    }
}