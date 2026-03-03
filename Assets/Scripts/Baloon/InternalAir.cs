using UnityEngine;
using UnityEngine.AI;

namespace Baloon
{
    public class InternalAir : Singleton<InternalAir>
    {
        [SerializeField]
        AnimationCurve altitudeAirDiffCurve;

        /// <summary>
        /// The difference between the internal air and the external air.
        /// </summary>
        float inExtDiff = 0f; 
        public float TemperatureDifference => inExtDiff;

        float decreaseSpeed = .5f;

        float increaseSpeed = 1f;

        
        float maxAltitude = 100;

        float maxTemperatureDifference = 25f;

        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.C))
            {
                decreaseSpeed = 5;
            }
            else
            {
                decreaseSpeed = 1f;
            }
#endif


            var curveValue = altitudeAirDiffCurve.Evaluate(transform.position.y / maxAltitude);

            var targetDiff = BoilerController.Instance.Power * maxTemperatureDifference * curveValue;

            var speed = decreaseSpeed;
                
            if(targetDiff > inExtDiff)
                speed = increaseSpeed;

            inExtDiff = Mathf.MoveTowards(inExtDiff, targetDiff, speed*Time.deltaTime);

            //inExtDiff -= decreaseSpeed * Time.deltaTime;
            
            if (inExtDiff < 0) inExtDiff = 0;
        }

        
    }
}