using DG.Tweening;
using System;
using System.Globalization;
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

        bool activated = false;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ResetText();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            BaloonControlPanel.OnStarted += HandleOnStarted;
            BaloonControlPanel.OnStopped += HandleOnStopped;
        }

        private void OnDisable()
        {
            BaloonControlPanel.OnStarted -= HandleOnStarted;
            BaloonControlPanel.OnStopped -= HandleOnStopped;
        }

        private void HandleOnStarted()
        {
            activated = true;
        }

        private void HandleOnStopped()
        {
            activated = false;
            ResetText();
        }

        private void LateUpdate()
        {
            if (activated)
            {
                var targetDiff = InternalAir.Instance.TargetTemperatureDifference;
                var diff = InternalAir.Instance.TemperatureDifference;
                var maxDiff = InternalAir.Instance.MaxTemperatureDifference;



                // Target arrow
                var targetAngle = Mathf.Lerp(minAngle, maxAngle, targetDiff / maxDiff);
                targetArrow.localEulerAngles = Vector3.down * targetAngle;

                targetAngle = Mathf.Lerp(minAngle, maxAngle, diff / maxDiff);
                arrow.localEulerAngles = Vector3.down * targetAngle;

                targetValue.text = targetDiff.ToString("00.00", CultureInfo.InvariantCulture);
                currentValue.text = diff.ToString("00.00", CultureInfo.InvariantCulture);



                // Arrows

                var lPos = Vector3.Lerp(leftStart.localPosition, leftStop.localPosition, targetDiff / maxDiff);
                leftArrow.localPosition = Vector3.SmoothDamp(leftArrow.localPosition, lPos, ref leftVelocity, .125f);
                lPos = Vector3.Lerp(rightStart.localPosition, rightStop.localPosition, diff / maxDiff);
                rightArrow.localPosition = Vector3.SmoothDamp(rightArrow.localPosition, lPos, ref rightVelocity, .125f);
            }
            else
            {
                var lPos = Vector3.Lerp(leftStart.localPosition, leftStop.localPosition, 0f);
                leftArrow.localPosition = Vector3.SmoothDamp(leftArrow.localPosition, lPos, ref leftVelocity, .125f);
                lPos = Vector3.Lerp(rightStart.localPosition, rightStop.localPosition, 0f);
                rightArrow.localPosition = Vector3.SmoothDamp(rightArrow.localPosition, lPos, ref rightVelocity, .125f);
            }
            

        }

        void ResetText()
        {
            targetValue.text = "--.--";
            currentValue.text = "--.--";
        }


    }
}