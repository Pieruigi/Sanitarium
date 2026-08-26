using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Baloon.UI
{
    public class StationReachedUI : Singleton<StationReachedUI>
    {
        [SerializeField]
        GameObject hintField;

        

        public bool IsVisible => hintField.activeSelf;

        protected override void Awake()
        {
            base.Awake();

            hintField.SetActive(false);
        }

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
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
            BaloonControlPanel.OnStopped += HandleOnStopped;
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
            BaloonControlPanel.OnStopped -= HandleOnStopped;
        }

        private void HandleOnStopped()
        {
            if (!IsVisible) return;

            hintField.SetActive(false);
        }

        private void HandleOnPathCleared()
        {
            if (IsVisible) return;

            string key = "fuel_hint";

            // Get the current launcher 
            var launchCtrl = FindObjectsByType<BaloonLauncherController>(FindObjectsSortMode.None).ToList().Find(l => l.Inside);
            if (!launchCtrl.GetComponentInParent<BaloonLauncher>().CompareTag("Fuel"))
            {
                key = "shut_down_hint";
            }


            hintField.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = key;

            hintField.SetActive(true);
        }
    }
}