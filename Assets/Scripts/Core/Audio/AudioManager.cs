using Helteix.Singletons.MonoSingletons;
using UnityEngine;

namespace OverBang.GameName.Core.Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private AudioPrefab audioPrefab;
        
        public AudioPrefab PlayAudio(AudioParameters parameters)
        {
            AudioPrefab instance = Instantiate(audioPrefab);
            instance.Initialize(parameters);
            return instance;
        }
    }
}