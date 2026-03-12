using System;
using UnityEngine;

namespace Baloon
{
    public class BoilerController : Singleton<BoilerController>
    {
        [SerializeField]
        [Range(0, 1f)]
        float throttle = 0;

        [SerializeField]
        HoldSlider throttleSlider;

        [SerializeField]
        HoldButton decreaseButton, increaseButton;

        //float power = 0;
        public float Power 
        {
            get
            {
                return throttle * maxPowers[version];
            }
        }

        float[] maxPowers = new float[] { 1f, 2f };

        int version = 0;

        public float MaxPower => maxPowers[version];

        

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKey(KeyCode.X))
            //{
            //    power = maxPowers[version];
            //}
            //else
            //{
            //    power = 0;
            //}
#endif

        }

        private void OnEnable()
        {
            throttleSlider.OnValueChanged += HandleOnThrottleSliderValueChanged;
            decreaseButton.OnPushed += HandleOnDecreasePushed;
            decreaseButton.OnReleased += HandleOnDecreaseReleased;
            increaseButton.OnPushed += HandleOnIncreasePushed;
            increaseButton.OnReleased += HandleOnIncreaseReleased;
        }

        private void OnDisable()
        {
            throttleSlider.OnValueChanged -= HandleOnThrottleSliderValueChanged;
            decreaseButton.OnPushed -= HandleOnDecreasePushed;
            decreaseButton.OnReleased -= HandleOnDecreaseReleased;
            increaseButton.OnPushed -= HandleOnIncreasePushed;
            increaseButton.OnReleased -= HandleOnIncreaseReleased;
        }

        private void HandleOnIncreasePushed()
        {
            throttle = Mathf.Clamp01(throttle+.01f);
        }

        private void HandleOnIncreaseReleased()
        {
            
        }

        private void HandleOnDecreasePushed()
        {
            throttle = Mathf.Clamp01(throttle - .01f);
        }

        private void HandleOnDecreaseReleased()
        {
            
        }

        private void HandleOnThrottleSliderValueChanged(float value)
        {
            throttle = value;
        }
    }

}
