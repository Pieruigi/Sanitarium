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

        public void SetPath(int pathIndex)
        {
            Debug.Log("TEST - Setting path:" + pathIndex);
            if(pathIndex < 0)
            {
                OnPathUnknown?.Invoke();
                return;
            }

            if (paths[pathIndex].IsLocked)
            {
                OnPathLocked?.Invoke();
                return;
            }

            currentPath = paths[pathIndex];
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