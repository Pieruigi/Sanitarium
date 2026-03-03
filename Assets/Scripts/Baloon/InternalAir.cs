using UnityEngine;
using UnityEngine.AI;

namespace Baloon
{
    public class InternalAir : Singleton<InternalAir>
    {
        /// <summary>
        /// The difference between the internal air and the external air.
        /// </summary>
        float inExtDiff = 0f; 
        public float TemperatureDifference => inExtDiff;

        float decreaseSpeed = 1f;

        float increaseSpeed = 1f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            // Check boiler controller power
            var speed = increaseSpeed * BoilerController.Instance.Power;

            // Update temperature 
            inExtDiff += speed * Time.deltaTime;
        }

        
    }
}