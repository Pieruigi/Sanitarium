using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Baloon
{
    public class BaloonPathManager : Singleton<BaloonPathManager>
    {
        public static UnityAction OnPathLocked;
        public static UnityAction OnPathSet;
        public static UnityAction OnPathCleared;
        public static UnityAction OnPathUnknown;
        public static UnityAction OnPathReversed;

        [SerializeField]
        List<BaloonPath> paths;

        [SerializeField]
        List<Transform> roots;


        BaloonPath currentPath = null;
        public BaloonPath CurrentPath => currentPath;

        bool isPathReversed = false;
        public bool IsPathReversed => isPathReversed;

       
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if(roots != null && roots.Count > 0)
            {
                if(paths == null) paths = new List<BaloonPath>();
                paths.Clear();

                foreach (Transform root in roots)
                {
                    var waypoints = root.GetComponentsInChildren<BaloonWaypoint>();
                    BaloonPath bp = new BaloonPath();
                    foreach(BaloonWaypoint waypoint in waypoints)
                    {
                        bp.AddWaypoint(waypoint);
                    }
                    paths.Add(bp);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool HasPath()
        {
            return currentPath != null;
        }

        public void SetPath(BaloonPath path, bool reversed, bool islocked)
        {
            SetPath(paths.IndexOf(path), reversed, islocked);
        }

        public void SetPath(int pathIndex, bool reversed, bool locked)
        {
            Debug.Log("TEST - Setting path:" + pathIndex);
            if(pathIndex < 0)
            {
                OnPathUnknown?.Invoke();
                return;
            }

            if (locked)
            {
                OnPathLocked?.Invoke();
                return;
            }

            currentPath = paths[pathIndex];
            isPathReversed = reversed;
            Debug.Log("TEST - OnPathSet");
            OnPathSet?.Invoke();
        }

        public void ClearPath()
        {
            currentPath = null;
            OnPathCleared?.Invoke();
        }

        public int GetIndex(BaloonPath path)
        {
            return paths.IndexOf(path);
        }

        public bool TryGetCurrentPathIndex(out int index)
        {
            index = -1;
            if (CurrentPath == null) return false;
            index = GetIndex(CurrentPath);
            return true;
        }

        public void ReversePath()
        {

            if (currentPath == null) return;

            isPathReversed = !isPathReversed;

            OnPathReversed?.Invoke();
        }
        
    }
}