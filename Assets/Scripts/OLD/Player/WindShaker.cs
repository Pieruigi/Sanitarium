using DG.Tweening;
using UnityEngine;

namespace Baloon
{
    public class WindShaker : MonoBehaviour
    {
        bool shaking = false;

        float shakeTimeMin = 10;
        float shakeTimeMax = 20;
        

        float shakeTime = 0;

        bool running = false;


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
                        CameraShake.Instance.PlayWindShakeLight();
                        YawBalloonLight();
                        if(range == AltitudeRange.Yellow) shakeTime *= .8f;
                        break;
                    case AltitudeRange.Red:
                        CameraShake.Instance.PlayWindShakeStrong();
                        shakeTime *= .6f;
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