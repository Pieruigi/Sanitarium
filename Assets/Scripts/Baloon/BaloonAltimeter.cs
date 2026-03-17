using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Baloon
{
    public class BaloonAltimeter : MonoBehaviour
    {
        [SerializeField]
        TMP_Text minValue, maxValue, currentValue;



        [SerializeField]
        List<LightController> lights;

        bool activated = false;

        int redIndex = 0, yellowIndex = 1, greenIndex = 2;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SetLightOffAll();
            ResetAltitudeValueAll();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            //if (!activated) return;

            var currentAltitude = BaloonController.Instance.Altitude;
            var minAltitude = AltitudeManager.Instance.MinAltitude;
            var maxAltitude = AltitudeManager.Instance.MaxAltitude;
            var middleAltitude = (maxAltitude + minAltitude) / 2f;
            var range = (maxAltitude - minAltitude);
            var greenAmount = range * .4f;
            var greenMax = middleAltitude + greenAmount / 2f;
            var greenMin = middleAltitude - greenAmount / 2f;

            Debug.Log("TEST - GreenMin:" + greenMin);
            Debug.Log("TEST - GreenMax:" + greenMax);


            // Set altitude fields
            minValue.text = minAltitude.ToString("000", CultureInfo.InvariantCulture);
            maxValue.text = maxAltitude.ToString("000", CultureInfo.InvariantCulture);
            currentValue.text = currentAltitude.ToString("000.00", CultureInfo.InvariantCulture);


            if (currentAltitude < minAltitude || currentAltitude > maxAltitude) // Out of range
            {
                SwitchLightDataAll(redIndex);
            }
            else // In range
            {
                if(currentAltitude < greenMin || currentAltitude > greenMax)
                {
                    SwitchLightDataAll(yellowIndex);
                }
                else
                {
                    SwitchLightDataAll(greenIndex);
                }
            }
        }

        private void OnEnable()
        {
            BaloonControlPanel.OnStarted += HandleOnBaloonStarted;
            BaloonControlPanel.OnStopped += HandleOnBaloonStopped;
        }

        private void OnDisable()
        {
            BaloonControlPanel.OnStarted -= HandleOnBaloonStarted;
            BaloonControlPanel.OnStopped -= HandleOnBaloonStopped;
        }

        private void HandleOnBaloonStarted()
        {
            activated = true;

            SetLightOnAll(redIndex);
        }

        private void HandleOnBaloonStopped()
        {
            activated = false;

            // Lights off
            SetLightOffAll();
            ResetAltitudeValueAll();
        }

        

        void SetLightOffAll()
        {
            foreach (LightController light in lights)
                light.SetOn(false);
        }

        void SetLightOnAll(int dataIndex)
        {
            foreach (LightController light in lights)
            {
                light.SwitchData(dataIndex);
                light.SetOn(true);
            }
                
        }

        void SwitchLightDataAll(int dataIndex)
        {
            foreach (LightController light in lights)
            {
                light.SwitchData(dataIndex);
            }
        }

        void ResetAltitudeValueAll()
        {
            string s = "--";
            minValue.text = maxValue.text = currentValue.text = s;

        }
    }
}