using DG.Tweening;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.InputManagerEntry;

namespace Baloon
{
    public class WindShaker : Singleton<WindShaker>
    {
        bool shaking = false;

        float shakeTimeMin = 10;
        float shakeTimeMax = 20;
        

        float shakeTime = 0;

        bool running = false;

        public bool Running 
        { 
            get { return running; } 
            set { running = value; ResetShakeTime(); }
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        

        }

        void LateUpdate()
        {
            if (!running) return;

            shakeTime -= Time.deltaTime;
            if(shakeTime < 0)
            {
                shakeTime = Random.Range(shakeTimeMin, shakeTimeMax);
                // Get altitude range
                var range = AltitudeManager.Instance.GetCurrentRange();

                switch (range)
                {
                    case AltitudeRange.Green:
                    case AltitudeRange.Yellow:
                        CameraShake.Instance.PlayWindShakeLight(ResetShakeTime, ResetShakeTime);
                        BaloonShaker.Instance.ShakeLight();
                        WindAudio.Instance.FadeLightVolume(Random.Range(3.2f, 4f));
                        break;
                    case AltitudeRange.Red:
                        CameraShake.Instance.PlayWindShakeStrong(ResetShakeTime, ResetShakeTime);
                        BaloonShaker.Instance.ShakeHeavy();
                        WindAudio.Instance.FadeHeavyVolume(Random.Range(3.2f, 4f));
                        break;
                }


            }
        }

        private void OnEnable()
        {
            BaloonPathManager.OnPathSet += HandleOnPathSet;
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
            KillerWind.OnWarningStarted += HandleOnKillerWindWarningStarted;
            KillerWind.OnWarningStopped += HandleOnKillerWindWarningStopped;
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
            KillerWind.OnWarningStarted -= HandleOnKillerWindWarningStarted;
            KillerWind.OnWarningStopped -= HandleOnKillerWindWarningStopped;
        }

        private void HandleOnKillerWindWarningStarted()
        {
            running = false;
        }

        private void HandleOnKillerWindWarningStopped()
        {
            if (BaloonPathManager.Instance.CurrentPath != null) running = true;
        }

        void ResetShakeTime()
        {
            var range = AltitudeManager.Instance.GetCurrentRange();
            if (range == AltitudeRange.Yellow) shakeTime *= .8f;
            else shakeTime *= .6f;
        }

        private void HandleOnPathSet()
        {
            running = true;
            shakeTime = Random.Range(shakeTimeMin, shakeTimeMax);

        }

        private void HandleOnPathCleared()
        {
            running = false;
        }

      
        
    }
}
