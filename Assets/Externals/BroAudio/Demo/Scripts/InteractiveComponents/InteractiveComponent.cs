using UnityEngine;

namespace Ami.BroAudio.Demo
{
	public abstract class InteractiveComponent : MonoBehaviour
	{
		[SerializeField] protected InteractiveZone[] InteractiveZones = null;

		protected virtual bool ListenToInteractiveZone() => true;
		protected virtual bool IsTriggerOnce => false;
		protected InteractiveZone InteractiveZone;

		protected virtual void Awake()
		{
			if(ListenToInteractiveZone())
			{
				for (int index = 0; index < InteractiveZones.Length; index++)
				{
					InteractiveZone zone = InteractiveZones[index];
					zone.OnInZoneStateChanged += OnInZoneChanged;
				}
			}
		}

		protected virtual void OnDestroy()
		{
			for (int index = 0; index < InteractiveZones.Length; index++)
			{
				InteractiveZone zone = InteractiveZones[index];
				zone.OnInZoneStateChanged -= OnInZoneChanged;
			}
		}

		protected virtual void OnInZoneChanged(InteractiveZone zone, bool isInZone)
		{
			InteractiveZone = zone;
			
			if (isInZone && IsTriggerOnce)
			{
				for (int index = 0; index < InteractiveZones.Length; index++)
				{
					InteractiveZone intZone = InteractiveZones[index];
					intZone.OnInZoneStateChanged -= OnInZoneChanged;
				}
			}
		}
	} 
}
