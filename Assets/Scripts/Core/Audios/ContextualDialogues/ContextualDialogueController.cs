using System.Collections.Generic;
using OverBang.ExoWorld.Core.Characters;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    
    public class ContextualDialogueController : NetworkBehaviour
    {
        public static ContextualDialogueController Instance { get; private set; }
        private readonly List<CdQueued> _toRemove = new();

        private Dictionary<CharacterData, List<CdQueued>> lineQueue;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            foreach (var (character, queue) in lineQueue)
            {
                using (ListPool<CdQueued>.Get(out List<CdQueued> toRemove))
                {
                    for (int i = 0; i < queue.Count; i++)
                    {
                        CdQueued cd = queue[i];
                        cd.Tick(dt);
                        if (cd.IsOutDated())
                            toRemove.Add(cd);
                    }

                    foreach (CdQueued cd in toRemove)
                    {
                        TryRemoveContextualDialogue(cd.dialogue);
                    }
                }
            }
        }
        
        public bool TryAddContextualDialogue(ContextualDialogue dialogue)
        {
            CdQueued cdd = new CdQueued();
            cdd.Initialize(dialogue, 20f);
            
            List<CdQueued> queue = lineQueue[dialogue.character];

            foreach (CdQueued cd in queue)
            {
                if (Equals(cdd.dialogue, cd.dialogue))
                {
                    Debug.Log("DialogueAlreadyInQueue,being killed");
                    return false;
                }
            }
            
            lineQueue[dialogue.character].Add(cdd);
            return true;
        }

        public bool TryRemoveContextualDialogue(ContextualDialogue dialogue )
        {
            List<CdQueued> queue = lineQueue[dialogue.character];

            foreach (CdQueued cd in queue)
            {
                if (Equals(dialogue, cd.dialogue))
                {
                    lineQueue[dialogue.character].Remove(cd);
                    return true;
                }
            }
            
            Debug.Log("Dialogue Not Found, already ended ?");
            return false;
        }
    }
}