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

        float decreaseSpeed = 1f;

        float increaseSpeed = .5f;

        
        float maxAltitude = 350;

        float maxTemperatureDifference = 10f;

        

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
                decreaseSpeed = .1f;
            }
#endif


            var curveValue = altitudeAirDiffCurve.Evaluate(transform.position.y / (maxAltitude * BoilerController.Instance.MaxPower));

            var targetDiff = BoilerController.Instance.Power * maxTemperatureDifference * curveValue;
            //targetDiff = Mathf.Clamp(targetDiff, 0, maxTemperatureDifference);


            var speed = decreaseSpeed;
                
            if(targetDiff > inExtDiff)
                speed = increaseSpeed;

            inExtDiff = Mathf.MoveTowards(inExtDiff, targetDiff, speed*Time.deltaTime);

            inExtDiff = Mathf.MoveTowards(inExtDiff, targetDiff, increaseSpeed * Time.deltaTime);
            inExtDiff -= decreaseSpeed * Time.deltaTime;

            if (inExtDiff < 0) inExtDiff = 0;
        }

        
    }
}