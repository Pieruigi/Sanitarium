using Baloon;
using System;
using System.Linq;
using TMM;
using UnityEngine;
using UnityEngine.Events;

//[System.Serializable]
//public struct BaloonLauncherDirections
//{
//    [SerializeField]
//    public int north;
//    [SerializeField]
//    public int east;
//    [SerializeField]
//    public int south;
//    [SerializeField]
//    public int west;

//    public BaloonLauncherDirections(int north = -1, int east = -1, int south = -1, int west = -1)
//    {
//        this.north = north;
//        this.east = east;
//        this.south = south;
//        this.west = west;
//    }
//}


[System.Serializable]
public class BaloonLaunchData
{
    [SerializeField]
    int pathIndex = -1;
    public int PathIndex => pathIndex;

    [SerializeField]
    bool locked;
    public bool Locked => locked;

    [SerializeField]
    bool reversed;
    public bool Reversed => reversed;

}





public class BaloonLauncher : MonoBehaviour
{
    public delegate void DirectionChangedDelegate(BaloonLauncher baloonLauncher);
    public static DirectionChangedDelegate OnDirectionChanged;


    ///// <summary>
    ///// Path indices (-1 means no path).
    ///// </summary>
    //[SerializeField]
    //BaloonLauncherDirections directions;
    [SerializeField]
    BaloonLaunchData[] directions = new BaloonLaunchData[4];

    //[SerializeField]
    //Vector4 directions = Vector4.one * -1;

    [SerializeField]
    int initialDirection = 0;

    //[SerializeField]
    //float launchForce = 3f;

    int currentDirection;
    public int CurrentDirection => currentDirection;

    [SerializeField]
    BaloonWaypoint waypoint;

    
    //int[] internalDirections;

    private void Awake()
    {
        //internalDirections = new int[] { (int)directions.x, (int)directions.y, (int)directions.z, (int)directions.w };
        currentDirection = initialDirection;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        //if (Input.GetKeyDown(KeyCode.X))
        //    SetPathFromCurrentDirection();
#endif
    }

    private void OnEnable()
    {
        BaloonPathManager.OnPathSet += HandleOnPathSet;
        BaloonPathManager.OnPathLocked += HandleOnPathLocked;
        BaloonPathManager.OnPathCleared += HandleOnPathCleared;
        BaloonPathManager.OnPathUnknown += HandleOnPathUnknown;
    }

    private void OnDisable()
    {
        BaloonPathManager.OnPathSet -= HandleOnPathSet;
        BaloonPathManager.OnPathLocked -= HandleOnPathLocked;
        BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
        BaloonPathManager.OnPathUnknown -= HandleOnPathUnknown;
    }

    private void HandleOnPathUnknown()
    {
        
        Debug.Log("TEST - OnPathUnknown");
    }

    private void HandleOnPathSet()
    {
     
        var currentPath = BaloonPathManager.Instance.CurrentPath;
        var reversed = BaloonPathManager.Instance.IsPathReversed;

        // Get internal waypoint
        //var waypoint = GetComponentInChildren<BaloonWaypoint>();

        if((reversed && currentPath.Waypoints.First() == waypoint) || (!reversed && currentPath.Waypoints.Last() == waypoint))
        {
            // is the destination
            // get the path index
            var pathIndex = BaloonPathManager.Instance.GetIndex(currentPath);
            // Get the corresponding path index in the launch data
            var direction = directions.ToList().FindIndex(d => d.PathIndex == pathIndex);

            currentDirection = direction;
            OnDirectionChanged?.Invoke(this);
        }

        Debug.Log("TEST - OnPathSet");
    }

    private void HandleOnPathLocked()
    {
        
        Debug.Log("TEST - OnPathLocked");
    }

    private void HandleOnPathCleared()
    {
        
        Debug.Log("TEST - OnPathCleared");
    }

    

    public void SwitchDirection()
    {
        Debug.Log("Switching direction");     
        //int length = internalDirections.Length;
        
        //for(int i=1; i<length; i++)
        //{
        //    int next = (currentDirection + i) % length;

        //    if (internalDirections[next] >= 0)
        //    {
        //        currentDirection = next;
        //        Debug.Log("New direction " + currentDirection);
        //        OnDirectionChanged?.Invoke(this);
        //        return;
        //    }
        
        //}

        
        currentDirection = (currentDirection + 1) % directions.Length;

        OnDirectionChanged?.Invoke(this);

    }

    public void SwitchDirection(int newDirection)
    {
        if(currentDirection == newDirection) return;    

        currentDirection = newDirection;
        OnDirectionChanged?.Invoke(this);
    }

    public void SetPathFromCurrentDirection()
    {
     
        BaloonLaunchData data = directions[currentDirection];
        BaloonPathManager.Instance.SetPath(data.PathIndex, data.Reversed, data.Locked);
    }

    public bool IsPathAvailable(int direction)
    {
        return directions[direction].PathIndex >= 0;
    }

    public bool HasPath(int pathIndex, bool reversed)
    {
        return directions.ToList().Exists(p=>p.PathIndex == pathIndex && p.Reversed == reversed);
    }
   
}
