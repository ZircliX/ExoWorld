using UnityEngine;

namespace OverBang.GameName.Core.Audio
{
    public class AudioPrefab : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        
        public void Initialize(AudioParameters parameters)
        {
            transform.position = parameters.Position;
            audioSource.clip = parameters.AudioClip;
            audioSource.volume = parameters.Volume;
            audioSource.pitch = parameters.Pitch;
            audioSource.spatialBlend = parameters.SpatialBlend;
            audioSource.loop = parameters.Loop;
            audioSource.Play();
            
            if (!parameters.Loop)
            {
                Destroy(gameObject, parameters.AudioClip.length / parameters.Pitch);
            }
        }
    }
}