using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Baloon
{
    /// <summary>
    /// Unlike the launch fan this fan only works on waypoints
    /// </summary>
    public class BaloonWaypointFan : MonoBehaviour
    {
    
        public static List<BaloonWaypointFan> fans = new List<BaloonWaypointFan>();

        [SerializeField]
        BaloonWaypoint waypoint;
        
        [SerializeField]
        Transform root;
 
        [SerializeField]
        Transform directPathPoint, reversedPathPoint;

        Sequence sequence;

        bool isActive;

        float heightDefault;

        bool isPlaying = false;


        private void Awake()
        {
            heightDefault = transform.position.y;

            var pos = root.position;
            pos.x = directPathPoint.position.x;
            pos.z = directPathPoint.position.z;
            root.position = pos;
            root.rotation = directPathPoint.rotation;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if (!isActive) return;

            var pos = root.position;
            pos.y = BaloonController.Instance.transform.position.y;
            root.position = pos;

            

        }

        private void OnEnable()
        {
            // Add to static list
            fans.Add(this);

            BaloonPathManager.OnPathSet += HandleOnPathSet;
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
            BaloonWaypoint.OnReached += HandleOnWaypointReached;
            BaloonPathManager.OnPathReversed += HandleOnPathReversed;

            SceneManager.sceneUnloaded += HandleSceneUnloaded;
        }

        private void OnDisable()
        {
            // Remove from static list
            fans.Remove(this);

            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
            BaloonWaypoint.OnReached -= HandleOnWaypointReached;
            BaloonPathManager.OnPathReversed -= HandleOnPathReversed;

            SceneManager.sceneUnloaded -= HandleSceneUnloaded;
        }

        private void HandleOnPathReversed()
        {
            if(!isActive) return; // Mean we are on a different path

            //var target = directPathPoint;
            //if (BaloonPathManager.Instance.IsPathReversed)
            //    target = reversedPathPoint;

            AdjustOrientation();

            //if()
        }

        private void HandleSceneUnloaded(Scene arg0)
        {
            fans.Clear();
        }

        private void HandleOnWaypointReached(BaloonWaypoint newWaypoint)
        {


            ////if(waypoint == NavigationSystem.Instance.WaypointB)
            //if(waypoint != newWaypoint)
            //{
            //    if (isPlaying && BaloonWaypointFan.HasFan(newWaypoint))
            //    {
                    
            //        isPlaying = false;
            //        // Shut down this fan
            //        transform.DOKill();
            //        transform.DOLocalRotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
            //            .SetEase(Ease.OutBack);
            //    }
                
            //}
            //else
            //{
            //    isPlaying = true;
            //    // Start rotating
            //    transform.DOKill();
            //    transform.DOLocalRotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
            //        .SetLoops(-1, LoopType.Incremental)
            //        .SetEase(Ease.InQuad);
            //}
        }

        private void HandleOnPathSet()
        {
                
            //if(BaloonPathManager.Instance.GetIndex(BaloonPathManager.Instance.CurrentPath) == pathindex)
            if(BaloonPathManager.Instance.CurrentPath.Waypoints.Contains(waypoint))
            {
                isActive = true;

                AdjustOrientation();
                StartRotating();
               
                
            }
        }

        private void HandleOnPathCleared()
        {
            isActive = false;
            if (sequence != null) sequence.Kill();
            root.DOKill();
            root.DOMoveY(heightDefault, 1f).SetEase(Ease.OutBack);
            transform.DOKill();
        }

        void StartRotating()
        {
            if (isPlaying) return;
            isPlaying = true;
            // Start rotating
            transform.DOKill();
            transform.DOLocalRotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.InQuad);
        }

        void AdjustOrientation()
        {
            var target = directPathPoint;
            if (BaloonPathManager.Instance.IsPathReversed)
                target = reversedPathPoint;

            if (sequence != null) sequence.Kill();
                root.DOKill();
                //transform.DOKill();

                sequence = DOTween.Sequence();
                sequence.Append(root.DOMoveX(target.position.x, 1f).SetEase(Ease.OutBack));
                sequence.Join(root.DOMoveZ(target.position.z, 1f).SetEase(Ease.OutBack));
                sequence.Join(root.DORotateQuaternion(target.rotation, 1f).SetEase(Ease.OutBack));
        }

        public static bool IsWaypointConnected(BaloonWaypoint waypoint)
        {
            return fans.Exists(f => f.waypoint == waypoint);
        }


        
    }
}