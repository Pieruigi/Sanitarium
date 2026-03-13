using Unity.VisualScripting;
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

        
        float maxAltitude = 350 * 2; // 647 actually

        float maxTemperatureDifference = 10f;
        public float MaxTemperatureDifference => maxTemperatureDifference;

        float targetTemperatureDifference = 0f;
        public float TargetTemperatureDifference => targetTemperatureDifference;

        bool coolerOn = false;


        bool _test = false;

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

            if (Input.GetKeyDown(KeyCode.X))
            {
                _test = true;
            }
#endif


            var curveValue = altitudeAirDiffCurve.Evaluate(transform.position.y / (maxAltitude * BoilerController.Instance.MaxPower));

            targetTemperatureDifference = !coolerOn ? BoilerController.Instance.Power * maxTemperatureDifference * curveValue : 0;
         
            //targetTemperatureDifference = Mathf.Round(targetTemperatureDifference * 4f) / 4f;

            if (_test) targetTemperatureDifference = 2.05f;

            var transitionSpeed = targetTemperatureDifference > inExtDiff ? increaseSpeed : decreaseSpeed;

            
            //if (targetDiff > inExtDiff)
            //    inExtDiff = Mathf.MoveTowards(inExtDiff, targetDiff, increaseSpeed * Time.deltaTime);
            //else
            //    inExtDiff = Mathf.MoveTowards(inExtDiff, targetDiff, decreaseSpeed * Time.deltaTime); 
            inExtDiff = Mathf.Lerp(inExtDiff, targetTemperatureDifference, transitionSpeed * Time.deltaTime);
            //inExtDiff = Mathf.Round(inExtDiff * 10f) / 10f;
        }

        
    }
}