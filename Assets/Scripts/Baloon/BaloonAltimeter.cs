using System;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class BaloonAltimeter : MonoBehaviour
    {
        [SerializeField]
        LightController middleLight;

        [SerializeField]
        List<LightController> topLights;

        [SerializeField]
        List<LightController> bottomLights;

        bool activated = false;

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
            if (!activated) return;

            var currentAltitude = BaloonController.Instance.Altitude;
            var requiredAltitude = AltitudeManager.Instance.Altitude;
            var range = AltitudeManager.Instance.Range;

            if(currentAltitude < requiredAltitude + range * .25f && currentAltitude > requiredAltitude - range * .25f)
            {
                middleLight.SwitchData(1);
                SetTopLightOffAll();
                SetBottomLightOffAll();
                
            }
            else
            {
                middleLight.SwitchData(0);

                if (currentAltitude < requiredAltitude)
                {
                    SetBottomLightOffAll();

                    if(currentAltitude < requiredAltitude - range * .5f)
                        topLights[0].SetOn(true);
                }
                else
                {
                    SetTopLightOffAll();

                    if(currentAltitude > requiredAltitude + range * .5f)
                        bottomLights[0].SetOn(true);
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

            middleLight.SwitchData(0);
            middleLight.SetOn(true);
        }

        private void HandleOnBaloonStopped()
        {
            activated = false;

            // Lights off
            SetLightOffAll();
        }

        void SetLightOffAll()
        {
            middleLight.SetOn(false);
            for (int i = 0; i < topLights.Count; i++)
            {
                topLights[i].SetOn(false);
                bottomLights[i].SetOn(false);
            }
            
        }

        void SetTopLightOffAll()
        {
            foreach (LightController light in topLights)
                light.SetOn(false);
        }

        void SetBottomLightOffAll()
        {
            foreach (LightController light in bottomLights)
                light.SetOn(false);
        }
    }
}