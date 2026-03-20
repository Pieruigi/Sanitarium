using Baloon;
using System;
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
    Vector4 directions = Vector4.one * -1;

    [SerializeField]
    int initialDirection = 0;

    [SerializeField]
    float launchForce = 3f;

    int currentDirection;
    public int CurrentDirection => currentDirection;

    int[] internalDirections;

    private void Awake()
    {
        internalDirections = new int[] { (int)directions.x, (int)directions.y, (int)directions.z, (int)directions.w };
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
        if (Input.GetKeyDown(KeyCode.X))
            SetPathFromCurrentDirection();
#endif
    }

    private void RegisterPathManagerEvents()
    {
        //BaloonPathManager.OnPathSet += HandleOnPathSet;
        //BaloonPathManager.OnPathLocked += HandleOnPathLocked;
        //BaloonPathManager.OnPathCleared += HandleOnPathCleared;
        //BaloonPathManager.OnPathUnknown += HandleOnPathUnknown;
    }

    private void UnregisterPathManagerEvents()
    {
        //BaloonPathManager.OnPathSet -= HandleOnPathSet;
        //BaloonPathManager.OnPathLocked -= HandleOnPathLocked;
        //BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
        //BaloonPathManager.OnPathUnknown -= HandleOnPathUnknown;
    }

    private void HandleOnPathUnknown()
    {
        UnregisterPathManagerEvents();
        Debug.Log("TEST - OnPathUnknown");
    }

    private void HandleOnPathSet()
    {
        UnregisterPathManagerEvents();
        Debug.Log("TEST - OnPathSet");
    }

    private void HandleOnPathLocked()
    {
        UnregisterPathManagerEvents();
        Debug.Log("TEST - OnPathLocked");
    }

    private void HandleOnPathCleared()
    {
        UnregisterPathManagerEvents();
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

        currentDirection = (currentDirection + 1) % internalDirections.Length;

        OnDirectionChanged?.Invoke(this);

    }

    public void SetPathFromCurrentDirection()
    {
        RegisterPathManagerEvents();
        BaloonPathManager.Instance.SetPath(internalDirections[currentDirection]);
    }
   
}
