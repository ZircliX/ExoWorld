using System.Collections.Generic;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.Database;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    public static class ContextualDialogueManager
    {
        private static Dictionary<string, ContextualDialogueData> loadedData;

        public static ContextualDialogueController Controller { get; private set; }
        
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

        public static void RegisterController(this ContextualDialogueController controller)
        {
            if (Controller == null)
            {
                Controller = controller;
            }
        }
        
        
        public static void FireEvent(this ContextualDialogueData data, CDContext context) => FireEvent(data.ID, context);
        
        public static void FireEvent(string id, CDContext context)
        {
            if (!GameDatabase.Global.TryGetAssetByID(context.characterDataId, out CharacterData characterData)) return;
            //Debug.Log("Get character data : " + characterData);
            
            if (TryGetDialogue(characterData, id, out ContextualDialogue contextualDialogue))
            {
                //Debug.Log("Get Dialogue");
                if (!Controller.TryAddContextualDialogue(contextualDialogue, context))
                {
                    Debug.LogWarning($"Cannot add {contextualDialogue} to controller !");
                }
            }
        }


        public static bool TryGetDialogueData(string id, out ContextualDialogueData dialogue)
        {
            return loadedData.TryGetValue(id, out dialogue);
        }
        public static bool TryGetDialogue(CharacterData from, string id, out ContextualDialogue dialogue)
        {
            if (loadedData.TryGetValue(id, out ContextualDialogueData contextualDialogueData))
            {
                if (contextualDialogueData.TryGetClip(from, out ContextualClip.CharacterLine line))
                {
                    dialogue = new ContextualDialogue(from, contextualDialogueData, line);
                    return true;
                }
            }
            
            dialogue = default;
            return false;
        }
    }
}