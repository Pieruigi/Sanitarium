using Baloon.SaveSystem;
using System;
using TMM;
using UnityEngine;

namespace Baloon
{
    public class BasePlatform : MonoBehaviour
    {
        public delegate void LandingDelegate(BasePlatform platform);
        public static LandingDelegate OnLanding;

        public delegate void TakeOffDelegate(BasePlatform platform);
        public static TakeOffDelegate OnTakeOff;

        public static BasePlatform CurrentPlatform { get; private set; }

        //[SerializeField]
        //ActivationTrigger trigger;

        bool inside;

        bool landingWithEngineOn = false;
        bool pathCleared = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
                        
        }

        private void OnEnable()
        {
            BaloonControlPanel.OnStopped += HandleOnStopped;
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
        }

        private void OnDisable()
        {
            BaloonControlPanel.OnStopped -= HandleOnStopped;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
        }

        private void HandleOnPathCleared()
        {
            pathCleared = true;
        }

        private void HandleOnStopped()
        {
            if (!landingWithEngineOn || !pathCleared) return;

            landingWithEngineOn = false;
            pathCleared = false;

            if (BoilerController.Instance.GasLeft > 0)
                SaveManager.Instance.Save();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Baloon")) return;
            HandleOnEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Baloon")) return;
            
            HandleOnExit(other);
        }

        private void HandleOnEnter(Collider other)
        {
            inside = true;
            CurrentPlatform = this;

            // You can save only when you enter the platform and the engine is running
            landingWithEngineOn = false;
            var c = BaloonController.Instance.GetComponentInChildren<BaloonControlPanel>();
            if (c.IsRunning)
            {
                landingWithEngineOn = true;
            }

            OnLanding?.Invoke(this);
        }

        private void HandleOnExit(Collider other)
        {
            

            inside = false;
            CurrentPlatform = null;

            // You can't save anymore here
            landingWithEngineOn = false;


            OnTakeOff?.Invoke(this);
        }
    }
}