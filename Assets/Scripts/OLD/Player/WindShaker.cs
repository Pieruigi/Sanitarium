using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Baloon
{
    public class WindShaker : Singleton<WindShaker>
    {
       
        float shakeTimeMin = 3.5f * 1.4f / BaloonController.SpeedMultiplier;
        float shakeTimeMax = 5f * 1.4f / BaloonController.SpeedMultiplier;
        

        float shakeTime = 0;

        bool running = false;

        public bool Running 
        { 
            get { return running; } 
            set { running = value; shakeTime = Random.Range(shakeTimeMin, shakeTimeMax); /* ResetShakeTime();*/ }
        }

        float lightStrength = 2.5f;
        float heavyStrength = 5f;

        float timeSpeed = 1f;// 1f;

        //public bool _testBalloonShaker = false;

        protected override void Awake()
        {
            base.Awake();

#if DEMO
            //shakeTimeMin = 3.5f * 1.4f;
            //shakeTimeMax = 5f * 1.4f;


#endif
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        

        }

#if UNITY_EDITOR
        private void Update()
        {
            
                
        }
#endif

        void LateUpdate()
        {
            if (!running ) return;

            AdjustTimeSpeed();

            shakeTime -= Time.deltaTime * timeSpeed;
            if(shakeTime < 0)
            {
                //shakeTime = Random.Range(shakeTimeMin, shakeTimeMax);
                shakeTime = 99999;
                //ResetShakeTime();
                // Get altitude range
                var range = AltitudeManager.Instance.GetCurrentRange();
               

                bool heavy = false;

                switch (range)
                {
                    case AltitudeRange.Green:
                        if (Random.Range(0, 6) == 0) // 1/6 heavy wind
                            heavy = true;
                        //ShakeLight();
                        break;
                    case AltitudeRange.Yellow:
                        if(Random.Range(0, 3) == 0) // 1/3 heavy wind
                            heavy = true;
                        
                        //CameraShake.Instance.PlayWindShakeLight(ResetShakeTime, ResetShakeTime);
                        //BaloonShaker.Instance.ShakeLight();
                        //WindAudio.Instance.FadeLightVolume(Random.Range(3.2f, 4f));
                        break;
                    case AltitudeRange.Red:
                        heavy = true;
                        //CameraShake.Instance.PlayWindShakeStrong(ResetShakeTime, ResetShakeTime);
                        //BaloonShaker.Instance.ShakeHeavy();
                        //WindAudio.Instance.FadeHeavyVolume(Random.Range(3.2f, 4f));
                        break;
                }

               

                // Shake
                if (heavy)
                    ShakeHeavy();
                else
                    ShakeLight();
                

            }
        }

        private void OnEnable()
        {
            BaloonPathManager.OnPathSet += HandleOnPathSet;
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
            KillerWind.OnKilling += HandleOnKillerWindWarningStarted;
           
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
            KillerWind.OnKilling -= HandleOnKillerWindWarningStarted;
           
        }

        void AdjustTimeSpeed()
        {
            var range = AltitudeManager.Instance.GetCurrentRange();
            switch (range)
            {
                case AltitudeRange.Green:
                    timeSpeed = 1f;
                    break;
                case AltitudeRange.Yellow:
                    timeSpeed = 1.4f;
                    break;
                case AltitudeRange.Red: 
                    timeSpeed = 1.8f;
                    break;
            }
        }

        float ComputeWindStrength(float currentAltitude, bool light)
        {
            const float minH = 20f;
            const float maxH = 120f;
            float minW = light ? lightStrength : heavyStrength;
            float maxW = minW * 1.5f;
            // 1. Calculate the rate of change (Wind units per Meter)
            // In this case: (1.5 - 0.5) / (120 - 30) = 1.0 / 90 = ~0.011
            float windPerMeter = (maxW - minW) / (maxH - minH);

            // 2. Apply the slope starting from the base point (30m, 0.5W)
            // This works for 10m, 200m, or any value.
            var windStrength = minW + (currentAltitude - minH) * windPerMeter;

            // Optional: Safety check to avoid negative wind if altitude goes below zero
            if (windStrength < 0) windStrength = 0;

            return windStrength;
        }

        void ShakeLight()
        {
            CameraShake.Instance.PlayWindShakeLight(ResetShakeTime, ResetShakeTime);
            //if(_testBalloonShaker)
            BaloonShaker.Instance.ShakeLight();
            WindAudio.Instance.FadeLightVolume(Random.Range(3.2f, 4f));

            var y = BaloonController.Instance.transform.position.y; ;
            y += ComputeWindStrength(BaloonController.Instance.Altitude, true) * (Random.Range(0, 2) == 0 ? 1 : -1) * Random.Range(.9f, 1.1f);
            BaloonController.Instance.transform.DOMoveY(y, 2f).SetEase(Ease.OutQuad);

            // Creek
            BaloonCreek.Instance.Play(Random.Range(4f, 5f), Random.Range(.4f, .6f));
        }

        void ShakeHeavy()
        {
            

            CameraShake.Instance.PlayWindShakeStrong(() => { ResetShakeTime(); StartCoroutine(ApplyDamage()); }, ResetShakeTime);
            //if (_testBalloonShaker)
            BaloonShaker.Instance.ShakeHeavy();
            WindAudio.Instance.FadeHeavyVolume(Random.Range(3.2f, 4f));

            var y = BaloonController.Instance.transform.position.y;
            y += ComputeWindStrength(BaloonController.Instance.Altitude, false) * (Random.Range(0, 2) == 0 ? 1 : -1) * Random.Range(.9f, 1.1f);
            BaloonController.Instance.transform.DOMoveY(y, 2f).SetEase(Ease.OutQuad);

            // Creek
            BaloonCreek.Instance.Play(Random.Range(4f, 5f), Random.Range(.6f, .8f));

            IEnumerator ApplyDamage()
            {
                
                if (Random.Range(0, 3) == 0) yield break; // 33% we take damage

                
                yield return new WaitForSeconds(.5f);

                if(BaloonBoilerHealth.Instance.TryTakeSingleDamage())
                    CameraShake.Instance.PlayJumpscare(1f);
            }
        }

        private void HandleOnKillerWindWarningStarted()
        {
            running = false;
        }

        void ResetShakeTime()
        {
           
            shakeTime = Random.Range(shakeTimeMin, shakeTimeMax);
            
            //var range = AltitudeManager.Instance.GetCurrentRange();
            //if (range == AltitudeRange.Yellow) shakeTime *= .8f;
            //else shakeTime *= .6f;
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
