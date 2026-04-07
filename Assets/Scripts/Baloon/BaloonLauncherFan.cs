using Baloon;
using DG.Tweening;
using System;
using System.Linq;
using TMM;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class BaloonLauncherFan : MonoBehaviour
{
    [SerializeField]
    ActivationTrigger activator;

    [SerializeField]
    Transform root;

    [SerializeField]
    Transform pivot;

    [SerializeField]
    BaloonWaypoint waypoint;

    bool inside = false;

    float followSpeed = 10;

    GameObject player;

    BaloonLauncher baloonLauncher;

    bool playing = false;

    private void Awake()
    {
        baloonLauncher = GetComponentInParent<BaloonLauncher>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        pivot.localEulerAngles = Vector3.up * 90f * baloonLauncher.CurrentDirection;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (!inside) return;

        var range = AltitudeManager.Instance.GetCurrentRange();
        switch (range)
        {
            case AltitudeRange.Green:
                var pos = root.position;
                pos.y = Mathf.Lerp(pos.y, player.transform.position.y, followSpeed * Time.deltaTime);
                root.position = pos;
                break;
        }
    }

    private void OnEnable()
    {
        activator.OnEnter += HandleOnEnter;
        activator.OnExit += HandleOnExit;
        BaloonLauncher.OnDirectionChanged += HandleOnDirectionChanged;
        BaloonPathManager.OnPathSet += HandleOnLaunched;
        BaloonPathManager.OnPathCleared += HandleOnPathCleared;
        BaloonWaypoint.OnReached += HandleOnWaypointReached;
        BaloonPathManager.OnPathReversed += HandleOnPathReversed;
    }

    private void OnDisable()
    {
        activator.OnEnter -= HandleOnEnter;
        activator.OnExit -= HandleOnExit;
        BaloonLauncher.OnDirectionChanged -= HandleOnDirectionChanged;
        BaloonPathManager.OnPathSet -= HandleOnLaunched;
        BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
        BaloonWaypoint.OnReached -= HandleOnWaypointReached;
        BaloonPathManager.OnPathReversed -= HandleOnPathReversed;
    }

    private void HandleOnPathReversed()
    {
        var path = BaloonPathManager.Instance.CurrentPath;
        var reversed = BaloonPathManager.Instance.IsPathReversed;
        var waypoints = BaloonPathManager.Instance.CurrentPath.Waypoints;

        var dest = !reversed ? waypoints.Last() : waypoints.First();
        var orig = !reversed ? waypoints.First() : waypoints.Last();

        if (waypoint == dest)
        {
            StopRotating();
        }
        else
        {
            // Get the current waypoints
            var a = NavigationSystem.Instance.WaypointA;
            var b = NavigationSystem.Instance.WaypointB;


            // Check if navigation system has already been updated
            //if (NavigationSystem.Instance.IsPathReversed != BaloonPathManager.Instance.IsPathReversed)
            //{
            //    // Switch waypoints
            //    var t = a;
            //    a = b;
            //    b = a;
            //}

            if(waypoint == a) 
                StartRotating();
        }




        //// Stop waypoint B fan is any
        //var w = waypoint;




    }

    private void HandleOnWaypointReached(BaloonWaypoint waypoint)
    {
        //if (!playing) return;
        //if (this.waypoint != waypoint && !BaloonWaypointFan.HasFan(waypoint)) return;
        //// Any waypoint
        //if (playing && this.waypoint != waypoint)
        //{
        //    StopRotating();
        //    //playing = false;
        //    //transform.DOKill();
        //    //transform.DOLocalRotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
        //    //.SetEase(Ease.OutBack);
        //}


    }

    private void HandleOnPathCleared()
    {
        playing = false;
        transform.DOKill(); 
    }

    private void HandleOnLaunched()
    {
        if (!inside) return;
        StartRotating(); 
        //playing = true;
        //transform.DOKill();
        //transform.DOLocalRotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
        //    .SetLoops(-1, LoopType.Incremental)
        //    .SetEase(Ease.InQuad);
    }

    private void HandleOnDirectionChanged(BaloonLauncher baloonLauncher)
    {
        if (this.baloonLauncher != baloonLauncher) return;

        // Get the current direction
        RotateToDirection(baloonLauncher.CurrentDirection);
        
        

    }

    private void HandleOnEnter(Collider other)
    {
        inside = true;
    }

    private void HandleOnExit(Collider other)
    {
        inside = false;
    }

    void StopRotating()
    {
        if (!playing) return;
        playing = false;
        transform.DOKill();
        transform.DOLocalRotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
        .SetEase(Ease.OutBack);
    }

    void StartRotating()
    {
        if (playing) return;
        playing = true;
        transform.DOKill();
        transform.DOLocalRotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.InQuad);
    }

    //void RotateToDirection(int direction)
    //{
    //    var currentY = pivot.eulerAngles.y;
    //    float targetY = 0;

    //    targetY = direction * 90f;

    //    targetY = (targetY % 360 + 360) % 360;

    //    // Keep clockwise
    //    while (targetY <= currentY)
    //        targetY += 360f;

    //    var duration = Mathf.Abs(currentY - targetY) / 90f;
    //    //duration *= .5f;

        
    //    pivot.DOKill();
    //    pivot.transform.DORotate(new Vector3(0, targetY, 0), duration, RotateMode.FastBeyond360).SetEase(Ease.OutBack, 1.2f);
    //}

    void RotateToDirection(int direction)
    {
        var currentY = pivot.eulerAngles.y;
        float targetY = direction * 90f;

        targetY = (targetY % 360 + 360) % 360;

        float angleDiff = Mathf.DeltaAngle(currentY, targetY);

        // Calculate a dynamic duration based on the actual distance to travel
        // Using 90 degrees as the baseline for 1 second of duration
        float duration = 1f;// Mathf.Abs(angleDiff) / 90f;

       

        pivot.DOKill();
        pivot.transform.DORotate(new Vector3(0, targetY, 0), duration, RotateMode.FastBeyond360).SetEase(Ease.OutBack, 1.2f);
    }
}
