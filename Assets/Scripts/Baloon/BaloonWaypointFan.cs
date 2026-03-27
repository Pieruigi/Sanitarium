using DG.Tweening;
using System;
using UnityEngine;

namespace Baloon
{
    /// <summary>
    /// Unlike the launch fan this fan only works on waypoints
    /// </summary>
    public class BaloonWaypointFan : MonoBehaviour
    {
    
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
            BaloonPathManager.OnPathSet += HandleOnPathSet;
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
            BaloonWaypoint.OnReached += HandleOnWaypointReached;
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
            BaloonWaypoint.OnReached -= HandleOnWaypointReached;
        }

        private void HandleOnWaypointReached(BaloonWaypoint newWaypoint)
        {
            //if(waypoint == NavigationSystem.Instance.WaypointB)
            if(waypoint != newWaypoint)
            {
                if (isPlaying)
                {
                    isPlaying = false;
                    // Shut down this fan
                    transform.DOKill();
                    transform.DOLocalRotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
                        .SetEase(Ease.OutBack);
                }
                
            }
            else
            {
                isPlaying = true;
                // Start rotating
                transform.DOKill();
                transform.DOLocalRotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Incremental)
                    .SetEase(Ease.InQuad);
            }
        }

        private void HandleOnPathSet()
        {
                
            //if(BaloonPathManager.Instance.GetIndex(BaloonPathManager.Instance.CurrentPath) == pathindex)
            if(BaloonPathManager.Instance.CurrentPath.Waypoints.Contains(waypoint))
            {
                isActive = true;

                var target = directPathPoint;
                if(BaloonPathManager.Instance.IsPathReversed)
                    target = reversedPathPoint;

                

                if (sequence != null) sequence.Kill();
                root.DOKill();
                transform.DOKill();

                sequence = DOTween.Sequence();
                sequence.Append(root.DOMoveX(target.position.x, 1f).SetEase(Ease.OutBack));
                sequence.Join(root.DOMoveZ(target.position.z, 1f).SetEase(Ease.OutBack));
                sequence.Join(root.DORotateQuaternion(target.rotation, 1f).SetEase(Ease.OutBack));

                
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
    }
}