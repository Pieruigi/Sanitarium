using DG.Tweening;
using System;
using UnityEngine;

namespace Baloon
{
    public class BaloonPowerLight : MonoBehaviour
    {
        [SerializeField]
        LightController greenLight;

        [SerializeField]
        LightController redLight;

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
            redLight.SetOn(false);
            greenLight.SetOn(true);
        }

        private void HandleOnBaloonStopped()
        {
            redLight.SetOn(true);
            greenLight.SetOn(false);
        }
    }
}