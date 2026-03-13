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

        float[] maxPowers = new float[] { 1f, 1.5f };

        int version = 0;

        public float MaxPower => maxPowers[version];

        float step = 0.01f;

        float speed = 0;
        float pushElapsed = 0f;

        float speed1 = 2f;
        float speed2 = 8f;
        float speed3 = 32f;

        float speedElapsed = 0f;

        int dir = 0;

        

        
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

            if (dir == 0) return;

            pushElapsed += Time.deltaTime;
            var pushTime = 1f;
            if(pushElapsed > pushTime)
            {
                if(speed < speed2)
                    speed = speed2;
                else if(speed < speed3)
                    speed = speed3;

                pushElapsed -= pushTime;
            }

            speedElapsed += Time.deltaTime;
            float speedTime = 1f / speed;
            if(speedElapsed > speedTime)
            {
                speedElapsed -= speedTime;
                throttle += step * dir;
                throttle = Mathf.Clamp01(throttle);
            }

            


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
            pushElapsed = 0;
            speedElapsed = 0;
            speed = speed1;
            dir = 1;
            throttle = Mathf.Clamp01(throttle+.01f);
        }

        private void HandleOnIncreaseReleased()
        {
            dir = 0;
        }

        private void HandleOnDecreasePushed()
        {
            pushElapsed = 0;
            speedElapsed = 0;
            speed = speed1;
            dir = -1;
            throttle = Mathf.Clamp01(throttle - .01f);
        }

        private void HandleOnDecreaseReleased()
        {
            dir = 0;
        }

        private void HandleOnThrottleSliderValueChanged(float value)
        {
            throttle = value;
        }
    }

}
