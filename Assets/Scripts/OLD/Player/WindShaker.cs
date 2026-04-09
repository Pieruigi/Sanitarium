using DG.Tweening;
using System.Collections;
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

        //public bool _testBalloonShaker = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        

        }

#if UNITY_EDITOR
        private void Update()
        {
            //if(Input.GetKeyDown(KeyCode.X))            
            //    ShakeLight();
            //if (Input.GetKeyDown(KeyCode.C))
            //    ShakeHeavy();
        }
#endif

        void LateUpdate()
        {
            if (!running) return;

            shakeTime -= Time.deltaTime;
            if(shakeTime < 0)
            {
                shakeTime = Random.Range(shakeTimeMin, shakeTimeMax);
                // Get altitude range
                var range = AltitudeManager.Instance.GetCurrentRange();

                bool heavy = false;

                switch (range)
                {
                    case AltitudeRange.Green:
                        ShakeLight();
                        break;
                    case AltitudeRange.Yellow:
                        if(Random.Range(0, 5) == 0) // 20% heavy wind
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

        void ShakeLight()
        {
            CameraShake.Instance.PlayWindShakeLight(ResetShakeTime, ResetShakeTime);
            //if(_testBalloonShaker)
            BaloonShaker.Instance.ShakeLight();
            WindAudio.Instance.FadeLightVolume(Random.Range(3.2f, 4f));
        }

        void ShakeHeavy()
        {
            CameraShake.Instance.PlayWindShakeStrong(()=> { ResetShakeTime(); StartCoroutine(ApplyDamage()); }, ResetShakeTime);
            //if (_testBalloonShaker)
            BaloonShaker.Instance.ShakeHeavy();
            WindAudio.Instance.FadeHeavyVolume(Random.Range(3.2f, 4f));

            IEnumerator ApplyDamage()
            {
                if (Random.Range(0, 2) == 0) yield break; // 50% we take damage

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
