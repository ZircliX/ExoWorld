using System.Collections.Generic;
using Helteix.Singletons.SceneServices;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class AreaManager : SceneService<AreaManager>
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
                if (area.CheckForPlayers(PlayerManager.Instance.GetPlayerTransforms()))
                {
                    availableSpawnAreas.Add(area);
                }
            }
            return availableSpawnAreas;
        }
    }
}