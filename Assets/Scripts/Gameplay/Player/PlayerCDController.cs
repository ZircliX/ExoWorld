using OverBang.ExoWorld.Core.Audios.ContextualDialogues;
using OverBang.ExoWorld.Core.Characters;
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
            ulong id = OwnerClientId;
            RegisterPlayerRpc(id);
        }

        [Rpc(SendTo.Everyone)]
        private void RegisterPlayerRpc(ulong id)
        {
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
                // Choisir l'index de la ligne
                LocalGamePlayer player = GamePlayerManager.Instance.GetLocalPlayer();
                if (!ContextualDialogueManager.TryGetDialogueData(dialogue.ID, out ContextualDialogueData dialogueData)) return;
                
                // Vérifier si le clip existe pour ce personnage
                if (dialogueData.TryGetClip(player.CharacterData, out ContextualClip.CharacterLine _))
                {
                    // La ligne existe, maintenant on choisit un index aléatoire
                    // On suppose que les lignes sont accessibles, sinon on doit modifier ContextualDialogueData
                    // Pour l'instant, utiliser TryGetLineIndex helper
                    int lineIndex = Random.Range(0, GetLineCountForCharacter(dialogueData, player.CharacterData));
                    
                    //Debug.Log("Fire Audio, everyone : " + dialogue.CanBeHeardByEveryone);
                    if (dialogue.CanBeHeardByEveryone)
                    {
                        FireDialogueRpc(dialogue.ID, GetContext(), lineIndex);
                    }
                    else
                    {
                        CDContext context = GetContext();
                        ContextualDialogueManager.FireEvent(dialogue.ID, context, lineIndex);
                    }
                }
            }
        }
        
        private int GetLineCountForCharacter(ContextualDialogueData dialogueData, CharacterData characterData)
        {
            // Utiliser la réflexion ou une méthode helper pour obtenir le nombre de lignes
            // Pour l'instant, on utilise une approche simple : essayer d'obtenir chaque index jusqu'à échouer
            int count = 0;
            while (dialogueData.TryGetClip(characterData, count, out _))
            {
                count++;
            }
            return count;
        }
        
        [Rpc(SendTo.Everyone)]
        private void FireDialogueRpc(string id, CDContext context, int lineIndex)
        {
            ContextualDialogueManager.FireEvent(id, context, lineIndex);
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