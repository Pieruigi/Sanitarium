using UnityEngine;

namespace Baloon
{
    public class BaloonWaypoint : MonoBehaviour
    {
        [SerializeField]
        float horizontalForce;
        public float HorizontalForce => horizontalForce;

        [SerializeField]
        float minAltitude, maxAltitude;
        public float MinAltitude => minAltitude;
        public float MaxAltitude => maxAltitude;


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