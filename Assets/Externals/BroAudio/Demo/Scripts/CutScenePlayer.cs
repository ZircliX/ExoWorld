using UnityEngine;
using UnityEngine.Playables;

namespace Ami.BroAudio.Demo
{
	public class CutScenePlayer : InteractiveComponent
	{
		[SerializeField] PlayableDirector _director = null;
		[SerializeField] SoundID _backgroundMusic = default;
		[SerializeField, Volume(true)] float _maxBgmVolumeDuringCutScene = default;

		protected override bool IsTriggerOnce => true;

		protected override void OnInZoneChanged(InteractiveZone zone,bool isInZone)
		{
			base.OnInZoneChanged(zone, isInZone);

			_director.Play();
			_director.stopped += OnCutSceneStopped;
            BroAudio.Play(_backgroundMusic)
                .AsBGM()
                .SetVolume(_maxBgmVolumeDuringCutScene);
		}

        private void OnCutSceneStopped(PlayableDirector director)
		{
			_director.stopped -= OnCutSceneStopped;
			BroAudio.SetVolume(_backgroundMusic,1f,2f);
		}
	}
}