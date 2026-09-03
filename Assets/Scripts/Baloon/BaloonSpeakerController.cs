using System;
using System.Linq;
using UnityEngine;

namespace Baloon
{
    public class BaloonSpeakerController : MonoBehaviour
    {
        [SerializeField]
        AudioClip landAndShutDownClip;

        [SerializeField]
        AudioClip landAndRefuelClip;

        [SerializeField]
        AudioClip noFuelClip;


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
            BaloonControlPanel.OnStarted += HandleOnStarted;
            BoilerController.OnGasDepleted += HandleOnGasDepleted;
            
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
            BaloonControlPanel.OnStarted -= HandleOnStarted;
            BoilerController.OnGasDepleted -= HandleOnGasDepleted;
        }

        private void HandleOnGasDepleted()
        {
            HandleOnStarted();
        }

        private void HandleOnStarted()
        {
            if(BoilerController.Instance.GasLeft == 0)
            {
                PlayNoFuelClip();
            }
        }

        private void HandleOnPathCleared()
        {
            // Get the current launcher
            var launcher = FindObjectsByType<BaloonLauncherController>(FindObjectsSortMode.None).ToList().Find(l => l.Inside);

            if (!launcher.GetComponentInParent<BaloonLauncher>().CompareTag("Fuel"))
            {
                GetComponent<BaloonSpeaker>().Play(landAndShutDownClip);
            }
            else
            {
                Debug.Log("TEST - Land and refuel");
                GetComponent<BaloonSpeaker>().Play(landAndRefuelClip);
            }

        }

        void PlayNoFuelClip()
        {
            var speaker = GetComponent<BaloonSpeaker>();
            if (!speaker.IsPlaying()) speaker.Play(noFuelClip);
        }
    }
}