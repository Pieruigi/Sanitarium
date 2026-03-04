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

        bool coolerOn = false;


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
                coolerOn = true;
            }
            else
            {
                decreaseSpeed = .5f;
                coolerOn = false;
            }
#endif


            var curveValue = altitudeAirDiffCurve.Evaluate(transform.position.y / (maxAltitude * BoilerController.Instance.MaxPower));

            var targetDiff = !coolerOn ? BoilerController.Instance.Power * maxTemperatureDifference * curveValue : 0;


            var transitionSpeed = targetDiff > inExtDiff ? increaseSpeed : decreaseSpeed;

            
            //if (targetDiff > inExtDiff)
            //    inExtDiff = Mathf.MoveTowards(inExtDiff, targetDiff, increaseSpeed * Time.deltaTime);
            //else
            //    inExtDiff = Mathf.MoveTowards(inExtDiff, targetDiff, decreaseSpeed * Time.deltaTime); 
            inExtDiff = Mathf.Lerp(inExtDiff, targetDiff, transitionSpeed * Time.deltaTime);

        }

        
    }
}