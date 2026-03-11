using UnityEngine;

namespace Baloon
{
    public class InternalAirIndicator : MonoBehaviour
    {
        [SerializeField]
        Transform arrow, targetArrow;


        float minAngle = 0f;
        float maxAngle = 90f;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            var targetDiff = InternalAir.Instance.TargetTemperatureDifference;
            var diff = InternalAir.Instance.TemperatureDifference;
            var maxDiff = InternalAir.Instance.MaxTemperatureDifference;

            // Target arrow
            var targetAngle = Mathf.Lerp(minAngle, maxAngle, targetDiff / maxDiff);
            targetArrow.localEulerAngles = Vector3.down * targetAngle;

            targetAngle = Mathf.Lerp(minAngle, maxAngle, diff / maxDiff);
            arrow.localEulerAngles = Vector3.down * targetAngle;

        }
    }
}