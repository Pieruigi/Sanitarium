using UnityEngine;


namespace Baloon
{
    /// <summary>
    /// Use this trigger whenever you want to set the target altitude
    /// </summary>
    public class AltitudeSetter : MonoBehaviour
    {
        [SerializeField]
        float minAltitude, maxAltitude;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            AltitudeManager.Instance.SetAltitude(minAltitude, maxAltitude);
        }

        
    }
}