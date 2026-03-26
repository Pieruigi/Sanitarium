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

        [SerializeField]
        List<BaloonPath> paths;

        BaloonPath currentPath = null;
        public BaloonPath CurrentPath => currentPath;

        bool isPathReversed = false;
        public bool IsPathReversed => isPathReversed;

       
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool HasPath()
        {
            return currentPath != null;
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
        
    }
}