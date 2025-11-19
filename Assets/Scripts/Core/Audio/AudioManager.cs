using Helteix.Singletons.MonoSingletons;

namespace OverBang.GameName.Core
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