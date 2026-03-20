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
        public IList<BaloonWaypoint> Waypoints => waypoints.AsReadOnly();

        [SerializeField]
        bool isReversed = false;
        public bool IsReversed => isReversed;

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