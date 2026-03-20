using System;
using UnityEngine;

namespace Baloon
{
    public class NavigationSystem : Singleton<NavigationSystem>
    {
        public const float DefaultSpeed = 3f;

        BaloonPath currentPath;

        BaloonWaypoint waypointA, waypointB;

        
        float speed = DefaultSpeed;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            BaloonPathManager.OnPathSet += HandleOnPathSet;
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
        }

        private void HandleOnPathCleared()
        {
            currentPath = null;
        }

        private void HandleOnPathSet()
        {
            currentPath = BaloonPathManager.Instance.CurrentPath;
            
            
            waypointA = !currentPath.IsReversed ? currentPath.Waypoints[0] : currentPath.Waypoints[currentPath.Waypoints.Count-1];
            waypointB = !currentPath.IsReversed ? currentPath.Waypoints[1] : currentPath.Waypoints[currentPath.Waypoints.Count - 2];


        }
    }
}