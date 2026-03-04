using System.Collections.Generic;
using Helteix.Singletons.MonoSingletons;

namespace OverBang.ExoWorld.Gameplay.Enemies
{
    public class AreaManager : MonoSingleton<AreaManager>
    {
        public List<Area> Areas { get; private set; }
        
        private void Awake()
        {
            Areas = new List<Area>();
        }

        public void Register(Area area)
        {
            Areas.Add(area);
        }

        public void Unregister(Area area)
        {
            Areas.Remove(area);
        }

        public HashSet<Area> GetSpawnableAreas()
        {
            HashSet<Area> availableSpawnAreas = new HashSet<Area>(Areas.Count);
            
            foreach (Area area in Areas)
            {
                if (area.IsValid)
                {
                    availableSpawnAreas.Add(area);
                }
            }
            return availableSpawnAreas;
        }
    }
}