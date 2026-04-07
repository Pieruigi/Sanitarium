using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Baloon
{
    [System.Serializable]
    public class BaloonPath// : MonoBehaviour
    {
        [SerializeField]
        List<BaloonWaypoint> waypoints;
        public IList<BaloonWaypoint> Waypoints => waypoints.AsReadOnly();

        //[SerializeField]
        //bool isReversed = false;
        //public bool IsReversed => isReversed;

        //[SerializeField]
        //bool isLocked = false;
        //public bool IsLocked => isLocked;

        public BaloonPath() { waypoints = new List<BaloonWaypoint>(); }

        public void AddWaypoint(BaloonWaypoint waypoint)
        {
            waypoints.Add(waypoint);
        }

        
    }
}