using DG.Tweening;
using StarterAssets;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Baloon
{
    public class NavigationSystem : Singleton<NavigationSystem>
    {
        public const float DefaultSpeed = 3f;

        BaloonPath currentPath;
        bool isPathReversed;
        public bool IsPathReversed => isPathReversed;

        BaloonWaypoint waypointA, waypointB;
        public BaloonWaypoint WaypointA => waypointA;
        public BaloonWaypoint WaypointB => waypointB;

   
        Vector2 horizontalDirectionTarget = Vector2.zero;

        GameObject player;

        Tween horizontalForceTween;


        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //baloonController = FindFirstObjectByType<BaloonController>();
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {
            if(currentPath != null)
            {
                var baloonController = BaloonController.Instance;

                var direction = Vector3.ProjectOnPlane(waypointB.transform.position - baloonController.transform.position, Vector3.up);
                
                var hTargetDir = new Vector2(direction.x, direction.z).normalized;
                Debug.Log("TEST - target direction:" + hTargetDir);

                baloonController.HorizontalDirection =  Vector2.Lerp(baloonController.HorizontalDirection, hTargetDir, Time.deltaTime);
            }
        }

        private void OnEnable()
        {
            BaloonPathManager.OnPathSet += HandleOnPathSet;
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
            BaloonWaypoint.OnReached += ReportWaypointReached;
            BaloonPathManager.OnPathReversed += HandleOnPathReversed;
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
            BaloonWaypoint.OnReached -= ReportWaypointReached;
            BaloonPathManager.OnPathReversed -= HandleOnPathReversed;
        }

        private void HandleOnPathCleared()
        {
            currentPath = null;
            waypointA = null;
            waypointB = null;
        }

        private void HandleOnPathSet()
        {
            Debug.Log("TEST - Setting path");
            currentPath = BaloonPathManager.Instance.CurrentPath;

            isPathReversed = BaloonPathManager.Instance.IsPathReversed;

            // Set waypoints A and B 
            waypointA = !isPathReversed ? currentPath.Waypoints[0] : currentPath.Waypoints[currentPath.Waypoints.Count - 1];
            waypointB = !isPathReversed ? currentPath.Waypoints[1] : currentPath.Waypoints[currentPath.Waypoints.Count - 2];


            StartCoroutine(StartMovingDelayed(1.5f));
            

            IEnumerator StartMovingDelayed(float delay)
            {
                yield return new WaitForSeconds(delay);

                Debug.Log("TEST - HorizontalForce:" + waypointA.HorizontalForce);

                if (horizontalForceTween != null) horizontalForceTween.Kill();

                // Set wind force
                horizontalForceTween = DOTween.To(()=>BaloonController.Instance.HorizontalForce, x=>BaloonController.Instance.HorizontalForce = x, waypointA.HorizontalForce, 2f);

                // Set target altitude
                AltitudeManager.Instance.SetAltitude(waypointB.MinAltitude, waypointB.MaxAltitude);
            }
        }


        void HandleOnPathReversed()
        {
            // Current path doesn't change and we already know we must reverse the path
            isPathReversed = !isPathReversed;

         
            // Reverse waypoints
            var tmp = waypointA;
            waypointA = waypointB;
            waypointB = tmp;

         
            // Set target altitude
            AltitudeManager.Instance.SetAltitude(waypointB.MinAltitude, waypointB.MaxAltitude);

            // Adjust horizontal force 
            if (horizontalForceTween != null) horizontalForceTween.Kill();
            horizontalForceTween = DOTween.To(() => BaloonController.Instance.HorizontalForce, x => BaloonController.Instance.HorizontalForce = x, waypointA.HorizontalForce, 2f);

            // Inverse direction
            var direction = BaloonController.Instance.HorizontalDirection;
            direction *= -1;
            direction.y = 0; // To be sure

            DOTween.To(() => BaloonController.Instance.HorizontalDirection, x => BaloonController.Instance.HorizontalDirection = x, direction, 2f);

        }

        public void ReportWaypointReached(BaloonWaypoint waypoint)
        {
            if (waypointB != waypoint) return;

            var destination = !isPathReversed ? currentPath.Waypoints.Last() : currentPath.Waypoints.First();

            if(waypoint != destination) // Keep going
            {
                // Update A and B
                waypointA = waypoint;
                int index = currentPath.Waypoints.IndexOf(waypoint);
                waypointB = !isPathReversed ? currentPath.Waypoints[index+1] : currentPath.Waypoints[index-1];

                // Set target altitude
                AltitudeManager.Instance.SetAltitude(waypointB.MinAltitude, waypointB.MaxAltitude);

                // Adjust horizontal force
                if (horizontalForceTween != null) horizontalForceTween.Kill();
                horizontalForceTween = DOTween.To(() => BaloonController.Instance.HorizontalForce, x => BaloonController.Instance.HorizontalForce = x, waypointA.HorizontalForce, 2f);

            }
            else // Destination reached
            {
                // Store the destination waypoint
                var target = waypointB.transform;

                
                // Clear baloon path
                BaloonPathManager.Instance.ClearPath();

                // Reset baloon horizontal direction
                BaloonController.Instance.HorizontalDirection = Vector2.zero;

                // Store horizontal volocity
                var currentHorizontalVelocity = new Vector2(BaloonController.Instance.CurrentVelocity.x, BaloonController.Instance.CurrentVelocity.z);
                // Remove horizontal force
                BaloonController.Instance.HorizontalForce = 0f;
                // Reset the baloon horizontal velocity 
                BaloonController.Instance.ResetHorizontalVelocity();
                // Now compute the horizontal direction between target and baloon
                var direction = Vector3.ProjectOnPlane(target.position - BaloonController.Instance.transform.position, Vector3.up);
                // How much time it will take to cover that distance at the current velocity
                var t = (currentHorizontalVelocity.magnitude > .1f) ? ( 2f * direction.magnitude / currentHorizontalVelocity.magnitude ) : 1.5f;


                var baloon = BaloonController.Instance.transform;

                var tweenSpeed = currentHorizontalVelocity.magnitude;

                DOTween.To(() => tweenSpeed, x => tweenSpeed = x, 0f, t)
                    .SetEase(Ease.OutQuad)
                    .OnUpdate(() =>
                    {
                        // Creiamo il vettore velocità basato sulla velocità calata dal tween
                        Vector3 newVelocity = direction.normalized * tweenSpeed;

                        // Lo passiamo al controller (che a sua volta trascinerà il player)
                        BaloonController.Instance.SetHorizontalVelocity(new Vector2(newVelocity.x, newVelocity.z));

                        // English comment: Safety snap to target if we are extremely close
                        if (Vector3.Distance(transform.position, target.position) < 0.05f)
                        {
                            BaloonController.Instance.ResetHorizontalVelocity();
                        }

                    })
                    .OnComplete(() => {
                        BaloonController.Instance.ResetHorizontalVelocity();
                        // Snap to position
                        var pos = new Vector3(target.position.x, baloon.position.y, target.position.z);
                        baloon.position = pos;
                        
                    });





            }
        }


        
    }
}