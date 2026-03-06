using DG.Tweening;
using System;
using UnityEngine;

namespace Baloon
{
    public class BaloonPowerLight : MonoBehaviour
    {
        [SerializeField]
        LightController light;

        
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
            light.SwitchData(2);
        }

        private void HandleOnBaloonStopped()
        {
            light.SwitchData(0);
        }
    }
}