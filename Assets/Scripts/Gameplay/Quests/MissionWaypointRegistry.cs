using System.Collections.Generic;
using OverBang.ExoWorld.Gameplay.Quests;
using UnityEngine;

public static class MissionWaypointRegistry
{
    private static readonly Dictionary<string, MissionWaypoint> waypoints = new Dictionary<string, MissionWaypoint>();

    public static void Register(MissionWaypoint waypoint)
    {
        if (!string.IsNullOrEmpty(waypoint.WaypointId))
            waypoints[waypoint.WaypointId] = waypoint;
    }

    public static void Unregister(MissionWaypoint waypoint)
    {
        if (!string.IsNullOrEmpty(waypoint.WaypointId))
            waypoints.Remove(waypoint.WaypointId);
    }

    public static MissionWaypoint Get(string id)
    {
        Debug.Log(id);
        foreach (KeyValuePair<string, MissionWaypoint> way in waypoints)
        {
            Debug.Log(way.Key);
        }
        waypoints.TryGetValue(id, out MissionWaypoint waypoint);
        return waypoint;
    }
}