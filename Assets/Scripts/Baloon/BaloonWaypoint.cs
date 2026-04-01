using StarterAssets;
using System;
using System.Linq;
using TMM;
using Unity.VisualScripting;
using UnityEngine;

namespace Baloon
{
    public class BaloonWaypoint : MonoBehaviour
    {
        enum AltitudeType { Self, Parent, MinMax}

        public delegate void ReachedDelegate(BaloonWaypoint waypoint);
        public static ReachedDelegate OnReached;

        public delegate void LeftDelegate(BaloonWaypoint waypoint);
        public static LeftDelegate OnLeft;

        //public readonly float HorizontalRange = 3f;
        [SerializeField]
        AltitudeType altitudeType;

        [SerializeField]
        float horizontalForce;
        public float HorizontalForce => horizontalForce;

        [SerializeField]
        float minAltitude, maxAltitude;
        public float MinAltitude => minAltitude;
        public float MaxAltitude => maxAltitude;

        
        bool isActive = false;
        bool isOrigin = false;
        bool isDestination = false;
      
        FirstPersonController player;

        private void Awake()
        {
            float offset = 10;
            switch (altitudeType)
            {
                case AltitudeType.Self:
                    var h = transform.position.y;
                    minAltitude = h - offset;
                    maxAltitude = h + offset;
                    break;
                case AltitudeType.Parent:
                    h = transform.parent.position.y;
                    minAltitude = h - offset;
                    maxAltitude = h + offset;
                    break;
                case AltitudeType.MinMax:

                    break;
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = FindFirstObjectByType<FirstPersonController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if (!isActive) return;

            // Adjust y depending on the player
            var pos = transform.position;
            pos.y = player.transform.position.y;
            transform.position = pos;
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

        private void HandleOnPathSet()
        {
            var currentPath = BaloonPathManager.Instance.CurrentPath;
            var waypoints = currentPath.Waypoints;
            bool isReversed = BaloonPathManager.Instance.IsPathReversed;
            // Check if it's a waypoint of the current path
            if (waypoints.Contains(this)) isActive = true;

            if (isActive)
            {
                if ((!isReversed && waypoints.Last() == this) || (isReversed && waypoints.First() == this))
                    isDestination = true;
                else if ((!isReversed && waypoints.First() == this) || (isReversed && waypoints.Last() == this))
                    isOrigin = true;

              
            }
                


            Debug.Log($"TEST - {gameObject.name} - isActive:{isActive}, isDestination:{isDestination}");


        }

        private void HandleOnPathCleared()
        {
            isActive = false;
            isDestination = false;  
            isOrigin = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive || !other.CompareTag("Player")) return;

            //if (NavigationSystem.Instance.WaypointB == this)
            //{
            //    NavigationSystem.Instance.ReportWaypointReached(this);
            //}
            
            OnReached?.Invoke(this);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isActive || !other.CompareTag("Player")) return;

            OnLeft?.Invoke(this);
        }
    }
}