using DG.Tweening;
using UnityEngine;

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
                        YawBalloonLight();
                        WindAudio.Instance.FadeLightVolume(Random.Range(3.2f, 4f));
                        break;
                    case AltitudeRange.Red:
                        CameraShake.Instance.PlayWindShakeStrong(ResetShakeTime, ResetShakeTime);
                        YawBalloonHeavy();
                        WindAudio.Instance.FadeHeavyVolume(Random.Range(3.2f, 4f));
                        break;
                }


            }
        }

        private void OnEnable()
        {
            BaloonPathManager.OnPathSet += HandleOnPathSet;
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
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

        private void YawBalloonLight()
        {
            var balloon = BaloonController.Instance.transform;
            var angle = Random.Range(20f, 45f);
            var duration = Random.Range(3.2f, 4f);
            balloon.DOLocalRotate(new Vector3(0, angle, 0), duration)
                .SetEase(Ease.InOutSine);
        }

        private void YawBalloonHeavy()
        {
            var balloon = BaloonController.Instance.transform;
            var angleY = Random.Range(20f, 45f);
            var angleX = Random.Range(3f, 6f);
            var angleZ = Random.Range(3f, 6f);
            var duration = Random.Range(3.2f, 4f);

            balloon.DOLocalRotate(new Vector3(0f, angleY, 0f), duration);

            balloon.DOLocalRotate(new Vector3(angleX, 0f, angleZ), duration / 2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    ResetAngles();

                })
                .OnKill(() =>
                {
                    ResetAngles();
                });

            void ResetAngles()
            {
                var r = balloon.localEulerAngles;
                r.x = r.z = 0f;
                balloon.localEulerAngles = r;
            }
        }

        void Shake()
        {
            // Get altitude
            var range = AltitudeManager.Instance.GetCurrentRange();

            switch (range)
            {
                case AltitudeRange.Green:

                    break;
                case AltitudeRange.Yellow:

                    break;
                case AltitudeRange.Red:

                    break;
            }
        }

        
    }
}
