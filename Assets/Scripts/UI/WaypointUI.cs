using System.Linq;
using UnityEngine;

namespace Baloon.UI
{
    public class WaypointUI : MonoBehaviour
    {
        [SerializeField]
        BaloonWaypoint waypoint;

   
        public BaloonWaypoint Waypoint
        {
            get
            {
                if(waypoint == null)
                {
                    var w = FindObjectsByType<BaloonWaypoint>(FindObjectsSortMode.None).ToList().Find(w => w.gameObject.name.ToLower().StartsWith($"{gameObject.name.ToLower()}-") || 
                                                                                                           w.transform.parent.gameObject.name.ToLower().StartsWith($"{gameObject.name.ToLower()}-"));
                    var bw = w.GetComponentInChildren<BaloonWaypoint>();
                    waypoint = bw;
                }
                return waypoint;
            }
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