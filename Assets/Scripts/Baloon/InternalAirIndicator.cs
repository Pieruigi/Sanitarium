using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Baloon
{
    public class InternalAirIndicator : MonoBehaviour
    {
        [SerializeField]
        Transform arrow, targetArrow;

        [SerializeField]
        TMP_Text currentValue, targetValue;

        [SerializeField]
        Transform leftArrow, rightArrow;

        [SerializeField]
        Transform leftStart, leftStop, rightStart, rightStop;


        float minAngle = 0f;
        float maxAngle = 90f;

        Vector3 leftVelocity;
        Vector3 rightVelocity;


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

            targetValue.text = targetDiff.ToString("00.00");
            currentValue.text = diff.ToString("00.00");

            // Arrows

            var lPos = Vector3.Lerp(leftStart.localPosition, leftStop.localPosition, targetDiff / maxDiff);
            leftArrow.localPosition = Vector3.SmoothDamp(leftArrow.localPosition, lPos, ref leftVelocity, .125f);
            lPos = Vector3.Lerp(rightStart.localPosition, rightStop.localPosition, diff / maxDiff);
            rightArrow.localPosition = Vector3.SmoothDamp(rightArrow.localPosition, lPos, ref rightVelocity, .125f);

        }
    }
}