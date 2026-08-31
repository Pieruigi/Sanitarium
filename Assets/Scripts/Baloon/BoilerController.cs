using Baloon.SaveSystem;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Baloon
{
    public class BoilerController : Singleton<BoilerController>
    {
        public static UnityAction OnGasDepleted;

        [SerializeField]
        [Range(0, 1f)]
        float throttle = 0;

        [SerializeField]
        HoldSlider throttleSlider;

        //[SerializeField]
        //HoldButton decreaseButton, increaseButton;

        [SerializeField]
        [Range (0, 1f)]
        float gasLeft;
        public float GasLeft => gasLeft;


        string saveId = "boiler";

        class Data
        {
            public float gasLeft;
        }
        

        //float power = 0;
        public float Power 
        {
            get
            {
                return throttle * maxPower;
            }
        }

        float maxPower = 1f;

        //float[] maxPowers = new float[] { 1f, 1.5f };

        int version = 0;

        public float MaxPower => maxPower;

        //float step = 0.01f;

        //float speed = 0;
        //float pushElapsed = 0f;

        //float speed1 = 2f;
        //float speed2 = 8f;
        //float speed3 = 32f;

        //float speedElapsed = 0f;

        //int dir = 0;


        bool running = false;


        float gasDepleteMaxSpeed = 0.0047f * 1.4f * BaloonController.SpeedMultiplier;// 1.4f;//0.00625f;
        float gasDepleteMinSpeed = 0.00094f * 1.4f * BaloonController.SpeedMultiplier;// 1.4f;//0.00125f

        protected override void Awake()
        {
            base.Awake();

#if DEMO
            //gasDepleteMaxSpeed = 0.0047f * 2.0f;// 1.4f;//0.00625f;
            //gasDepleteMinSpeed = 0.00094f * 2.0f;// 1.4f;//0.00125f
            gasDepleteMaxSpeed *= 1.2f;// 1.4f;//0.00625f;
            gasDepleteMinSpeed *= 1.2f;// 1.4f;//0.00125f

#endif

#if UNITY_EDITOR
            //gasLeft = .125f;
#endif
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var rawData = SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData);
                gasLeft = data.gasLeft;
            }
        }

        // Update is called once per frame
        void Update()
        {


            // Only if you use buttons rather than handle
            //if (dir == 0) return;

            //pushElapsed += Time.deltaTime;
            //var pushTime = 1f;
            //if(pushElapsed > pushTime)
            //{
            //    if(speed < speed2)
            //        speed = speed2;
            //    else if(speed < speed3)
            //        speed = speed3;

            //    pushElapsed -= pushTime;
            //}

            //speedElapsed += Time.deltaTime;
            //float speedTime = 1f / speed;
            //if(speedElapsed > speedTime)
            //{
            //    speedElapsed -= speedTime;
            //    throttle += step * dir;
            //    throttle = Mathf.Clamp01(throttle);
            //}

            // The amount of gas you are using depends of the throttle
            if (!running) return;

            var amount = (throttle * gasDepleteMaxSpeed + gasDepleteMinSpeed) * Time.deltaTime;

#if UNITY_EDITOR
            //return;
#endif

            if(gasLeft > 0)
            {
                gasLeft -= amount;
                if (gasLeft < 0)
                {
                    gasLeft = 0;
                    OnGasDepleted?.Invoke();
                }
            }
            


            
            //if (gasLeft == 0) throttle = 0;

          
        }

        private void OnEnable()
        {
            throttleSlider.OnValueChanged += HandleOnThrottleSliderValueChanged;
            //decreaseButton.OnPushed += HandleOnDecreasePushed;
            //decreaseButton.OnReleased += HandleOnDecreaseReleased;
            //increaseButton.OnPushed += HandleOnIncreasePushed;
            //increaseButton.OnReleased += HandleOnIncreaseReleased;
            BaloonControlPanel.OnStarted += HandleOnPanelControlStarted;
            BaloonControlPanel.OnStopped += HandleOnPanelControlStopped;
            BaloonBoilerHealth.OnDamageTaken += HandleOnDamageTaken;
            BaloonBoilerHealth.OnRepaired += HandleOnRepaired;

            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
        }

        private void OnDisable()
        {
            throttleSlider.OnValueChanged -= HandleOnThrottleSliderValueChanged;
            //decreaseButton.OnPushed -= HandleOnDecreasePushed;
            //decreaseButton.OnReleased -= HandleOnDecreaseReleased;
            //increaseButton.OnPushed -= HandleOnIncreasePushed;
            //increaseButton.OnReleased -= HandleOnIncreaseReleased;
            BaloonControlPanel.OnStarted -= HandleOnPanelControlStarted;
            BaloonControlPanel.OnStopped -= HandleOnPanelControlStopped;
            BaloonBoilerHealth.OnDamageTaken -= HandleOnDamageTaken;
            BaloonBoilerHealth.OnRepaired -= HandleOnRepaired;

            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
        }

        private void HandleOnUpdateDataEntry()
        {
            var data = new Data();
            data.gasLeft = gasLeft;//  Mathf.Max(gasLeft, 0.2f);
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        private void HandleOnDamageTaken(float oldHealth, float newHealth)
        {
            //maxPower = newHealth;
            AdjustPowerByHealth(newHealth);
        }

        private void HandleOnRepaired(float oldHealth, float newHealth)
        {
            //maxPower = newHealth;
            AdjustPowerByHealth(newHealth);
        }

        void AdjustPowerByHealth(float health)
        {
            maxPower = Mathf.Lerp(.4f, 1f, health);
        }

        private void HandleOnPanelControlStarted()
        {
            running = true;
        }

        private void HandleOnPanelControlStopped()
        {
            running = false;
            throttle = 0;
        }

        //private void HandleOnIncreasePushed()
        //{
        //    pushElapsed = 0;
        //    speedElapsed = 0;
        //    speed = speed1;
        //    dir = 1;
        //    throttle = Mathf.Clamp01(throttle+.01f);
        //}

        //private void HandleOnIncreaseReleased()
        //{
        //    dir = 0;
        //}

        //private void HandleOnDecreasePushed()
        //{
        //    pushElapsed = 0;
        //    speedElapsed = 0;
        //    speed = speed1;
        //    dir = -1;
        //    throttle = Mathf.Clamp01(throttle - .01f);
        //}

        //private void HandleOnDecreaseReleased()
        //{
        //    dir = 0;
        //}

        private void HandleOnThrottleSliderValueChanged(float value)
        {
            if(running /*&& gasLeft > 0*/) throttle = value;
        }

        public bool TryRefuel(float value)
        {
            if (gasLeft == 1) return false;
            
            gasLeft += value;
            if (gasLeft > 1) gasLeft = 1; // Clamp

            return true;
        }

        public bool IsFull()
        {
            if (gasLeft > 1) gasLeft = 1; // To be sure
            return gasLeft == 1;
        }
    }

}
