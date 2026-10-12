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

        [SerializeField]
        AudioClip gainAltitudeClip;

        [SerializeField]
        AudioClip reduceAltitudeClip;

        float warningTime = 5f;
        float warningElapsed = 0;

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
            if (BaloonPathManager.Instance.HasPath())
            {
                if(AltitudeManager.Instance.GetCurrentRange() == AltitudeRange.Red)
                {
                    warningElapsed += Time.deltaTime;
                    if(warningElapsed > warningTime)
                    {
                        warningElapsed -= warningTime;
                        var altitude = BaloonController.Instance.Altitude;
                        AudioClip clip = gainAltitudeClip;
                        if (altitude > AltitudeManager.Instance.MaxAltitude)
                            clip = reduceAltitudeClip;
                        PlayAltitudeWarning(clip);
                    }
                }
                else
                {
                    warningElapsed = 0;
                }
            }
            
        }

        private void OnEnable()
        {
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
            BaloonControlPanel.OnStarted += HandleOnStarted;
            BoilerController.OnGasDepleted += HandleOnGasDepleted;
            BaloonPathManager.OnPathSet += HandleOnPathSet;
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
            BaloonControlPanel.OnStarted -= HandleOnStarted;
            BoilerController.OnGasDepleted -= HandleOnGasDepleted;
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
        }

        private void HandleOnPathSet()
        {
            warningElapsed = 0;
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

            // Get the speaker
            var speaker = GetComponent<BaloonSpeaker>();

            // If it's playing (altitude warning for example) then stop it 
            if (speaker.IsPlaying()) speaker.Stop();

            if (!launcher.GetComponentInParent<BaloonLauncher>().CompareTag("Fuel"))
            {
                speaker.Play(landAndShutDownClip);
            }
            else
            {
                //Debug.Log("TEST - Land and refuel");
                speaker.Play(landAndRefuelClip);
            }

        }

        void PlayNoFuelClip()
        {
            var speaker = GetComponent<BaloonSpeaker>();
            if (!speaker.IsPlaying()) speaker.Play(noFuelClip);
        }

        void PlayAltitudeWarning(AudioClip clip)
        {
            var speaker = GetComponent<BaloonSpeaker>();
            if (!speaker.IsPlaying()) speaker.Play(clip, speaker.VolumeDefault * 2f);
        }
    }
}