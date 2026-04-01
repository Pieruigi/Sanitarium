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

        bool landing = false;

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
            if (!activated) return;

            var currentAltitude = BaloonController.Instance.Altitude;
            var minAltitude = AltitudeManager.Instance.MinAltitude;
            var maxAltitude = AltitudeManager.Instance.MaxAltitude;
            
           
            // Set altitude fields
            minValue.text = minAltitude.ToString("000", CultureInfo.InvariantCulture);
            maxValue.text = maxAltitude.ToString("000", CultureInfo.InvariantCulture);
            currentValue.text = currentAltitude.ToString("000.00", CultureInfo.InvariantCulture);

            if (landing) return;

            AltitudeRange currentRange = AltitudeManager.Instance.GetCurrentRange();
            
            switch (currentRange)
            {
                case AltitudeRange.Red:
                    SwitchLightDataAll(redIndex);
                    break;
                case AltitudeRange.Yellow:
                    SwitchLightDataAll(yellowIndex);
                    break;
                case AltitudeRange.Green:
                    SwitchLightDataAll(greenIndex);
                    break;
            }

            
        }

        private void OnEnable()
        {
            BaloonControlPanel.OnStarted += HandleOnBaloonStarted;
            BaloonControlPanel.OnStopped += HandleOnBaloonStopped;
            BasePlatform.OnLanding += HandleOnLanding;
            BasePlatform.OnTakeOff += HandleOnTakeOff;
        }

        private void OnDisable()
        {
            BaloonControlPanel.OnStarted -= HandleOnBaloonStarted;
            BaloonControlPanel.OnStopped -= HandleOnBaloonStopped;
            BasePlatform.OnLanding -= HandleOnLanding;
            BasePlatform.OnTakeOff -= HandleOnTakeOff;
        }

        private void HandleOnLanding(BasePlatform platform)
        {
            landing = true;

            // Lights off
            SetLightOffAll();
            ResetAltitudeValueAll();
        }

        private void HandleOnTakeOff(BasePlatform platform)
        {
            landing = false;

            SetLightOnAll(redIndex);

            
        }

        private void HandleOnBaloonStarted()
        {
           activated = true;
        }

        private void HandleOnBaloonStopped()
        {
            activated = false;

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
            
            minValue.text = "---";
            maxValue.text = "---";
            currentValue.text = "---.--";
            
        }
    }
}