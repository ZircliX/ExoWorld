using System.Collections.Generic;
using Helteix.Singletons.SceneServices;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    
    public class ContextualDialogueController : SceneService<ContextualDialogueController>
    {
        private class CdQueuedComparer : IComparer<CdQueued>
        {
            public int Compare(CdQueued x, CdQueued y)
            {
                if (y != null && x != null )
                        return x.dialogue.priority.CompareTo(y.dialogue.priority);
                return 0;
            }
        }
        
        private SubtitlesManager subtitlesManager;
        private readonly CdQueuedComparer comparer = new CdQueuedComparer();
        private readonly Dictionary<ulong, List<CdQueued>> lineQueue = new Dictionary<ulong, List<CdQueued>>();

        private void OnEnable()
        {
            this.RegisterController();
        }

        public void RegisterManager(SubtitlesManager manager)
        {
            subtitlesManager = manager;
        }

        public void UnregisterManager(SubtitlesManager manager)
        {
            if (subtitlesManager == manager)
            {
                subtitlesManager = null;
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            foreach ((_, List<CdQueued> queue) in lineQueue)
            {
                if (queue.Count == 0)
                    continue;

                RemoveOutdatedCD(queue, dt);

                if (queue.Count == 0)
                    continue;

                CdQueued last = queue[^1];
                queue.Sort(comparer);

                ProcessFirstQueuedDialogue(last, queue);
            }
        }

        // ReSharper disable once InconsistentNaming
        private void RemoveOutdatedCD(List<CdQueued> queue, float dt)
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
                    TryRemoveContextualDialogue(cd.dialogue, cd.context);
            }
        }

        private void ProcessFirstQueuedDialogue(CdQueued last, List<CdQueued> queue)
        {
            if (queue.Count == 0) return;

            CdQueued cdQueued = queue[^1];

            if (!cdQueued.WasFired)
            {
                if (last != cdQueued && last.IsPlaying)
                {
                    TryRemoveContextualDialogue(cdQueued.dialogue, cdQueued.context);
                    return;
                }

                cdQueued.Fire();
                if (subtitlesManager != null)
                    subtitlesManager.DisplaySubtitle(cdQueued);
            }
            else if (cdQueued.IsFinished)
            {
                TryRemoveContextualDialogue(cdQueued.dialogue, cdQueued.context);
            }
        }

        public void RegisterPlayer(ulong playerId)
        {
            lineQueue.Add(playerId, new List<CdQueued>());
        }

        public void UnregisterPlayer(ulong playerId)
        {
            lineQueue.Remove(playerId);
        }
        
        public bool TryAddContextualDialogue(ContextualDialogue dialogue, CDContext context)
        {
            if (!lineQueue.TryGetValue(context.playerId, out List<CdQueued> queue))
            {
                lineQueue.TryGetValue(context.playerId, out List<CdQueued> test);
                Debug.Log(test);
                return false;
            }

            CdQueued cdd = new CdQueued(dialogue, context);

            foreach (CdQueued cd in queue)
            {
                if (cdd.dialogue.Equals(cd.dialogue))
                {
                    Debug.Log("DialogueAlreadyInQueue,being killed");
                    return false;
                }
            }
            
            lineQueue[context.playerId].Add(cdd);
            return true;
        }

        public bool TryRemoveContextualDialogue(ContextualDialogue dialogue, CDContext context)
        {
            if (!lineQueue.TryGetValue(context.playerId, out List<CdQueued> queue)) return false;

            foreach (CdQueued cd in queue)
            {
                if (Equals(dialogue, cd.dialogue))
                {
                    cd.Kill();
                    lineQueue[context.playerId].Remove(cd);
                    return true;
                }
            }
            
            Debug.Log("Dialogue Not Found, already ended ?");
            return false;
        }
    }

    public static class ContextualDialogueControllerExtensions
    {
        public static void RegisterManager(this SubtitlesManager manager)
        {
            ContextualDialogueController.Instance.RegisterManager(manager);
        }
        
        public static void UnregisterManager(this SubtitlesManager manager)
        {
            ContextualDialogueController.Instance.UnregisterManager(manager);
        }
    }
}