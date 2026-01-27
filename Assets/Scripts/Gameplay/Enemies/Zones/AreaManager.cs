using System.Collections.Generic;
using Helteix.Singletons.MonoSingletons;

namespace OverBang.ExoWorld.Gameplay.Enemies
{
    public class AreaManager : MonoSingleton<AreaManager>
    {
        private List<Area> areas;
        
        private void Awake()
        {
            areas = new List<Area>();
        }

        public void Register(Area area)
        {
            areas.Add(area);
        }

        public void Unregister(Area area)
        {
            areas.Remove(area);
        }

        public HashSet<Area> GetSpawnableAreas()
        {
            
            HashSet<Area> availableSpawnAreas = new HashSet<Area>(areas.Count);
            
            foreach (Area area in areas)
            {
                if (area.IsValid())
                {
                    availableSpawnAreas.Add(area);
                }
            }
            return availableSpawnAreas;
        }
    }
}