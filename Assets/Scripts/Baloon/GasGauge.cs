using System;
using UnityEngine;

namespace Baloon
{
    public class GasGauge : MonoBehaviour
    {
        [SerializeField]
        Transform arrow;

        [SerializeField]
        Transform emptyPoint, fullPoint;

        [SerializeField]
        LightController shortageLight;

        float shortageLimit = .25f;

        bool started = false;

        float arrowSpeed = 1f;

        private void Awake()
        {
            arrow.localPosition = emptyPoint.localPosition;
        }

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
            //if (!started) return;

            if(BoilerController.Instance.GasLeft < shortageLimit)
            {
                if(!shortageLight.IsOn && started) shortageLight.SetOn(true);
            }

            var pos = Vector3.Lerp(emptyPoint.localPosition, fullPoint.localPosition, BoilerController.Instance.GasLeft);
            arrow.localPosition = Vector3.Lerp(arrow.localPosition, pos, arrowSpeed * Time.deltaTime);
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
            started = true;
        }

        private void HandleOnStopped()
        {
            started = false;

            shortageLight.SetOn(false);
        }
    }
}