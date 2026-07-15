using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Baloon.UI
{


    public class PlayerMap : MonoBehaviour
    {
        [SerializeField]
        GameObject root;

        [SerializeField]
        List<WaypointUI> uiWaypoints;

        [SerializeField]
        GameObject balloonImage;

        bool open = false;

        bool hasPath = false;

        BaloonWaypoint prevWaypoint = null, nextWaypoint = null;
        WaypointUI uiPrevWaypoint = null, uiNextWaypoint = null;

        List<BaloonWaypoint> waypoints;



        private void Awake()
        {
            root.SetActive(false);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            waypoints = FindObjectsByType<BaloonWaypoint>(FindObjectsSortMode.None).ToList();

            prevWaypoint = GetTheClosestWaypoint();
            nextWaypoint = prevWaypoint;
            uiPrevWaypoint = uiWaypoints.Find(w => w.Waypoint == prevWaypoint);
            uiNextWaypoint = uiWaypoints.Find(w => w.Waypoint == nextWaypoint);

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!open)
                    Open();
                else
                    Close();
            }
            
        }

        private void LateUpdate()
        {
            if (!open) return;

            if (hasPath)
            {
                prevWaypoint = NavigationSystem.Instance.WaypointA;
                uiPrevWaypoint = uiWaypoints.Find(w=>w.Waypoint == prevWaypoint);
                nextWaypoint = NavigationSystem.Instance.WaypointB;
                uiNextWaypoint = uiWaypoints.Find(w => w.Waypoint == nextWaypoint);

                Debug.Log($"TEST - MAP - PrevWP:{prevWaypoint}");
                Debug.Log($"TEST - MAP - NextWP:{nextWaypoint}");
                Debug.Log($"TEST - MAP - PrevWP_UI:{uiPrevWaypoint}");
                Debug.Log($"TEST - MAP - NextWP_UI:{uiNextWaypoint}");
            }

            
            if (hasPath)
            {
                // Compute direction
                var uiDest = (uiNextWaypoint.transform as RectTransform).anchoredPosition;
                var uiOrig = (uiPrevWaypoint.transform as RectTransform).anchoredPosition;
                var dest = nextWaypoint.transform.position;
                var orig = prevWaypoint.transform.position;
                dest.y = orig.y = 0;
                var balloon = BaloonController.Instance.transform.position;
                balloon.y = 0;
                var dist = Vector3.Distance(dest, orig);
                var bDist = Vector3.Distance(orig, balloon);
                var ratio = bDist / dist;

                
                // Compute ui direction
                var uiDir = uiDest - uiOrig;
                uiDir.Normalize();
                // Compute distance from ui origin
                var uiDist = Mathf.Lerp(0, Vector2.Distance(uiOrig, uiDest), ratio);
                // Move to current ui position
                (balloonImage.transform as RectTransform).anchoredPosition = (uiPrevWaypoint.transform as RectTransform).anchoredPosition + uiDir * uiDist;

            }
            else
            {
                (balloonImage.transform as RectTransform).anchoredPosition = (uiPrevWaypoint.transform as RectTransform).anchoredPosition;
            }
            

            //Debug.Log($"TEST - UI - PrevWaypoint:{uiOrig}, NextWaypoint:{uiDest}");



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

        //private void HandleOnWaypointReached(BaloonWaypoint waypoint)
        //{
        //    if (waypoint != nextWaypoint) return;

        //    prevWaypoint = waypoint;
        //    uiPrevWaypoint = uiWaypoints.Find(w => w.Waypoint == prevWaypoint);

        //}

        private void HandleOnPathSet()
        {
            hasPath = true;
            nextWaypoint = NavigationSystem.Instance.WaypointB;
            uiNextWaypoint = uiWaypoints.Find(w => w.Waypoint == nextWaypoint);
        }

        private void HandleOnPathCleared()
        {
            hasPath = false;
            prevWaypoint = nextWaypoint;
            uiPrevWaypoint = uiWaypoints.Find(w => w.Waypoint == prevWaypoint);
        }

        void Open()
        {
            if (open) return;
            open = true;
            root.SetActive(open);
            hasPath = BaloonPathManager.Instance.HasPath();

            if (hasPath) 
            {
                // Update waypoints
                prevWaypoint = NavigationSystem.Instance.WaypointA;
                uiPrevWaypoint = uiWaypoints.Find(w => w.Waypoint == prevWaypoint);
                nextWaypoint = NavigationSystem.Instance.WaypointB;
                uiNextWaypoint = uiWaypoints.Find(w => w.Waypoint == nextWaypoint);
            }
            
        }

        void Close()
        {
            if (!open) return;
            open = false;
            root.SetActive(open);
        }

        BaloonWaypoint GetTheClosestWaypoint()
        {
            var bPos = BaloonController.Instance.transform.position;
            bPos.y = 0;
            foreach(var waypoint in waypoints)
            {
                var wPos = waypoint.transform.position;
                wPos.y = 0;
                var d = Vector3.Distance(bPos, wPos);
                if (d < 1f)
                    return waypoint;
            }

            return null;
        }
    }
}