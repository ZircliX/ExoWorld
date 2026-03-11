using System;
using OverBang.ExoWorld.Core.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    [CreateAssetMenu(menuName = "OverBang/Audios/ContextualDialogue")]
    public class ContextualDialogueData : ScriptableObject
    {
        [field: SerializeField, ReadOnly] public string ID {get; private set;}
        [field: SerializeField, Range(0, 5)] public int Priority { get; private set; } = 0;
        [field: SerializeField, Range(0, 1)] public float Probability { get; private set; } = 1;
        [field: SerializeField] public bool CanBeHeardByEveryone { get; private set; } = true;
        [field: SerializeField] public float VoiceLifetime { get; private set; } = 0.2f;

        [SerializeField, ListDrawerSettings(ShowFoldout = false)] 
        private ContextualClip[] clips;
        
        private void Reset()
        {
            GenerateID();
        }


        public bool TryGetClip(CharacterData data, out ContextualClip.CharacterLine line)
        {
            line = default;
            if (clips.Length == 0)
                return false;

            for (int i = 0; i < clips.Length; i++)
            {
                if(clips[i].CharacterData == data) 
                    return clips[i].TryGetLine(out line);
            }
            
            return false;
        }
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(ID))
            {
                ID = Guid.NewGuid().ToString();
                Debug.LogWarning($"[OnValidate] Regenerating ID for {name}..." +
                                 $"{ID}");
            }
            
            for (int i = 0; i < clips.Length; i++)
            {
                for (int j = 0; j < clips[i].Lines.Length; j++)
                {
                    ContextualClip.CharacterLine line = clips[i].Lines[j]; 
                    line.UpdateCharacterCount();    
                    clips[i].Lines[j] = line; 
                }
            }
        }
            
        [Button]
        private void GenerateID()
        {
            ID = Guid.NewGuid().ToString();
            Debug.LogWarning($"[OnGenerateID] ID  manually generated for {name}..." +
                             $"{ID}");
        }

    }
}