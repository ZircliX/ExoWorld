using UnityEngine;

namespace Ami.BroAudio.Demo
{
	public abstract class InteractiveComponent : MonoBehaviour
	{
		[SerializeField] protected InteractiveZone[] InteractiveZones;

		protected virtual bool ListenToInteractiveZone() => true;
		protected virtual bool IsTriggerOnce => false;

		protected virtual void Awake()
		{
			if(ListenToInteractiveZone())
			{
				for (int i = 0; i < InteractiveZones.Length; i++)
				{
					InteractiveZone zone = InteractiveZones[i];
					zone.OnInZoneStateChanged += OnInZoneChanged;
				}
			}
		}

		protected virtual void OnDestroy()
		{
			for (int i = 0; i < InteractiveZones.Length; i++)
			{
				InteractiveZone zone = InteractiveZones[i];
				zone.OnInZoneStateChanged -= OnInZoneChanged;
			}
		}

		public virtual void OnInZoneChanged(InteractiveZone zone, bool isInZone)
		{
			if(isInZone && IsTriggerOnce)
			{
				zone.OnInZoneStateChanged -= OnInZoneChanged;
			}
		}
	} 
}
