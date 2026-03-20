using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Baloon
{
    [System.Serializable]
    public class BaloonPath// : MonoBehaviour
    {
        [SerializeField]
        List<BaloonWaypoint> waypoints;

        [SerializeField]
        bool isReversed = false;

        [SerializeField]
        bool isLocked = false;
        public bool IsLocked => isLocked;

        private void Awake()
        {
            //waypoints = GetComponentsInChildren<BaloonWaypoint>().ToList();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        
    }
}