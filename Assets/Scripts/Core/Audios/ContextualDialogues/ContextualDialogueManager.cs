using System.Collections.Generic;
using OverBang.ExoWorld.Core.Characters;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    public static class ContextualDialogueManager
    {
        private static Dictionary<string, ContextualDialogueData> loadedData;

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            ContextualDialogueData[] r = Resources.LoadAll<ContextualDialogueData>("Audios/ContextualDialogue");
            loadedData = new Dictionary<string, ContextualDialogueData>();
            
            for (int i = 0; i < r.Length; i++)
            {
                ContextualDialogueData d = r[i];
                if (!loadedData.TryAdd(d.ID, d))
                    Debug.LogError($"Cannot add duplicate ID {d.ID} for contextual dialogue");
            }
        }


        public static void FireEvent(this ContextualDialogueData data, CDContext context) => FireEvent(data.ID, context);
        
        public static void FireEvent(string id, CDContext context)
        {
            if (TryGetEvent(context.data, id, out ContextualDialogue contextualDialogue))
            {
                if (!ContextualDialogueController.Instance.TryAddContextualDialogue(contextualDialogue))
                {
                    Debug.LogError($"Cannot add {contextualDialogue} to controller !");
                }
            }
        }


        public static bool TryGetEvent(CharacterData from, string id, out ContextualDialogue dialogue)
        {
            if (loadedData.TryGetValue(id, out ContextualDialogueData contextualDialogueData))
            {
                if (contextualDialogueData.TryGetClip(from, out var line))
                {
                    dialogue = new ContextualDialogue(contextualDialogueData, line);
                    return true;
                }
            }
            
            dialogue = default;
            return false;
        }
    }
}