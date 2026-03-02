using System;
using NUnit.Framework;
using OverBang.ExoWorld.Core.Characters;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    [CreateAssetMenu(menuName = "OverBang/Audios/ContextualDialogue")]
    public class ContextualDialogueData : ScriptableObject
    {
        [field: SerializeField] public string ID {get; private set;}
        [field: SerializeField] public bool CanBeHeardByEveryone { get; private set; }

        [SerializeField] 
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

            float rnd = Random.value;
            for (int i = 0; i < clips.Length; i++)
            {
                rnd -= clips[i].Probability;
                if (rnd <= 0)
                    return clips[i].TryGetLine(data, out line);
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

            using (DictionaryPool<int, float>.Get(out var dictionary))
            {
                float total = 0;
                for (int i = 0; i < clips.Length; i++)
                {
                    ContextualClip clip = clips[i];
                    total += clip.Probability;
                    
                    dictionary.Add(i, clip.Probability);
                }

                foreach ((int i, float prob) in dictionary)
                {
                    float normalizedProb = prob / total;
                    clips[i] = clips[i].SetProbability(normalizedProb);
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