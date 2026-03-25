using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Baloon
{
    public class NavigationSystem : Singleton<NavigationSystem>
    {
        public const float DefaultSpeed = 3f;

        BaloonPath currentPath;

        BaloonWaypoint waypointA, waypointB;
        public BaloonWaypoint WaypointA => waypointA;
        public BaloonWaypoint WaypointB => waypointB;


        BaloonController baloonController;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            baloonController = FindFirstObjectByType<BaloonController>();
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
            Debug.Log("TEST - Setting path");
            currentPath = BaloonPathManager.Instance.CurrentPath;
            

            // Set waypoints A and B 
            waypointA = !currentPath.IsReversed ? currentPath.Waypoints[0] : currentPath.Waypoints[currentPath.Waypoints.Count - 1];
            waypointB = !currentPath.IsReversed ? currentPath.Waypoints[1] : currentPath.Waypoints[currentPath.Waypoints.Count - 2];


            StartCoroutine(StartMovingDelayed(1.5f));
            

            IEnumerator StartMovingDelayed(float delay)
            {
                yield return new WaitForSeconds(delay);

                Debug.Log("TEST - HorizontalForce:" + waypointA.HorizontalForce);
                // Set wind force
                baloonController.HorizontalForce = waypointA.HorizontalForce;
                // Set wind direction
                var direction = Vector3.ProjectOnPlane(waypointB.transform.position - waypointA.transform.position, Vector3.up);
                baloonController.HorizontalDirection = new Vector2(direction.x, direction.z).normalized;

                // Set target altitude
                AltitudeManager.Instance.SetAltitude(waypointB.MinAltitude, waypointB.MaxAltitude);
            }
        }

        public void ReportWaypointReached(BaloonWaypoint waypoint)
        {
            var destination = !currentPath.IsReversed ? currentPath.Waypoints.Last() : currentPath.Waypoints.First();

            if(waypoint != destination)
            {
                // Update A and B
                waypointA = waypoint;
                int index = currentPath.Waypoints.IndexOf(waypoint);
                waypointB = !currentPath.IsReversed ? currentPath.Waypoints[index+1] : currentPath.Waypoints[index-1];

                var direction = Vector3.ProjectOnPlane(waypointB.transform.position - waypointA.transform.position, Vector3.up);
                baloonController.HorizontalDirection = new Vector2(direction.x, direction.z).normalized;
            }
        }

        
    }
}