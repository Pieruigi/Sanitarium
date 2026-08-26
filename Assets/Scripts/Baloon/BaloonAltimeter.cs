using DG.Tweening;
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
        TMP_Text minValue, maxValue, currentValue, targetValue;

        [SerializeField]
        List<LightController> lights;



        bool activated = false;

        bool landing = false;

        int redIndex = 0, yellowIndex = 1, greenIndex = 2;

        List<Tween> tweens = new List<Tween>();

        string baseColorPropName = "_BaseColor";


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
            var targetAltitude = (maxAltitude - minAltitude) * .5f + minAltitude;
           
            // Set altitude fields
            minValue.text = minAltitude.ToString("000", CultureInfo.InvariantCulture);
            maxValue.text = maxAltitude.ToString("000", CultureInfo.InvariantCulture);
            //targetValue.text = targetAltitude.ToString("<mspace=.5em>000</mspace>.<mspace=.5em>00</mspace>", CultureInfo.InvariantCulture);

            int integerPart = (int)targetAltitude;
            // Get the decimals (e.g., 0.456 -> 45)
            int decimalPart = (int)((targetAltitude - integerPart) * 100);

            // Compose the final string: 
            // The integer part and decimal part are inside <mspace>, but the dot is OUTSIDE.
            targetValue.text = $"<mspace=0.5em>{integerPart:D3}</mspace>.<mspace=0.5em>{decimalPart:D2}</mspace>";

            integerPart = currentAltitude >= 0 ? (int)currentAltitude : 0;
            // Get the decimals (e.g., 0.456 -> 45)
            decimalPart = currentAltitude >= 0 ? (int)((currentAltitude - integerPart) * 100) : 0;

            currentValue.text = $"<mspace=0.5em>{integerPart:D3}</mspace>.<mspace=0.5em>{decimalPart:D2}</mspace>";


            if (landing) return;

            AltitudeRange currentRange = AltitudeManager.Instance.GetCurrentRange();

            var hasPath = BaloonPathManager.Instance.HasPath();

            switch (currentRange)
            {
                case AltitudeRange.Red:
                    if (hasPath)
                    {
                        StopFlickering();
                        SwitchLightDataAll(redIndex);
                    }
                    else
                    {
                        SwitchLightDataAll(yellowIndex);
                        StartFlickering();
                    }
                    break;
                case AltitudeRange.Yellow:
                    StopFlickering();
                    SwitchLightDataAll(yellowIndex);
                    break;
                case AltitudeRange.Green:
                    StopFlickering();
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
            
            //minValue.text = "---";
            //maxValue.text = "---";
            targetValue.text = "";
            currentValue.text = "";
            
        }

        void StartFlickering()
        {
            if (tweens.Count > 0) return;

            
            Vector4 defaultColor = lights[0].GetBaseColorByIndex(1);

            tweens.Clear();

            foreach(LightController light in lights)
            {
                var tween = DOTween.Sequence();

                Vector4 startColor = defaultColor;

                tween.Append(DOTween.To(() => startColor, x => startColor = x, defaultColor, .5f));
                //tween.AppendCallback(() => { GetComponent<AudioSource>().Play(); });
                //tween.AppendInterval(1f);
                tween.Append(DOTween.To(() => startColor, x => startColor = x, defaultColor * .5f, .5f));
                tween.SetLoops(-1, LoopType.Restart);
                tween.OnUpdate(() =>
                {

                    light.ForceColor(startColor);
                    //GetComponent<Renderer>().SetPropertyBlock(lightController.MaterialPropertyBlock);
                }
                );
                tween.OnKill(() =>
                {
                    light.ForceColor((Vector4)defaultColor);
                    //tweens.Remove(tween);
                });
                //tween.OnComplete(() => { Debug.Log("TEST - On Complete..."); });
                tween.Play();
                tweens.Add(tween);

            }

        }

        void StopFlickering()
        {
            foreach (var t in tweens)
                t.Kill();
            tweens.Clear();
        }
    }
}