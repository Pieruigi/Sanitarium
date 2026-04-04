using StarterAssets;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Baloon
{
    public class KillerWind : Singleton<KillerWind>
    {
        public static UnityAction OnWarningStarted;
        public static UnityAction OnWarningStopped;
        

        [SerializeField]
        float safeZoneKillTime; // How much time in red range before wind kills you

        [SerializeField]
        float travelZoneKillTime;

        [SerializeField]
        AudioSource hauntingAudioSource;

        [SerializeField]
        AudioSource bangingAudioSource;

        [SerializeField]
        GameObject tentaclesPrefab;

        float killElapsed = 0;

        float killTime;

        bool running = false;

        float warningTimeLeft = 1.5f;
        bool warning = false;

        bool killing = false;

        FirstPersonController player;
        BaloonDestroyer balloonDestroyer;

        protected override void Awake()
        {
            base.Awake();


        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = FindFirstObjectByType<FirstPersonController>();
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
          

            //if (Input.GetKeyDown(KeyCode.C))
            //    StopWarningShake();
#endif
        }

        private void LateUpdate()
        {
            if (!running || killing) return;

            // Get the altitude range
            var range = AltitudeManager.Instance.GetCurrentRange();

            // Check
            switch (range)
            {
                case AltitudeRange.Green:
                case AltitudeRange.Yellow:
                    if(!warning)
                        killElapsed = 0;
                    //StopWarning();
                    break;
                case AltitudeRange.Red:
                    killElapsed += Time.deltaTime;

                    if(killElapsed > killTime - warningTimeLeft)
                    {
                        //StartWarning();
                    }

                    if (killElapsed > killTime)
                    {
                        // You die
                        StartKilling();
                    }
                    break;
            }

           
        }

        private void OnEnable()
        {
            BasePlatform.OnLanding += HandleOnLanding;
            BasePlatform.OnTakeOff += HandleOnTakeOff;
            BaloonPathManager.OnPathSet += HandleOnPathSet;
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
        }

        private void OnDisable()
        {
            BasePlatform.OnLanding -= HandleOnLanding;
            BasePlatform.OnTakeOff -= HandleOnTakeOff;
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
        }

        private void HandleOnPathSet()
        {
            killTime = travelZoneKillTime;
            warning = false;
        }

        private void HandleOnPathCleared()
        {
            killTime = safeZoneKillTime;
            warning = false;
        }

        private void HandleOnLanding(BasePlatform platform)
        {
            running = false;
            killTime = safeZoneKillTime;
            killElapsed = 0;
            warning = false;
        }

        private void HandleOnTakeOff(BasePlatform platform)
        {
            running = true;
            killTime = safeZoneKillTime;
            killElapsed = 0;
            warning = false;
        }

        void StartWarning()
        {
            if (warning) return;
            warning = true;

            // Start shaking the balloon
            BaloonShaker.Instance.StartWarningShake();
            WindAudio.Instance.FadeKillerVolume(warningTimeLeft);

            OnWarningStarted?.Invoke();
        }

        void StopWarning()
        {
            if (!warning) return;
            warning = false;

            BaloonShaker.Instance.StopWarningShake();
            WindAudio.Instance.FadeReset();

            OnWarningStopped?.Invoke();
        }

        void StartKilling()
        {
            if (killing) return;
            killing = true;

            StartCoroutine(SpawnTentacles());
            StartCoroutine(DoKill());
            
            IEnumerator DoKill()
            {
                hauntingAudioSource.Play();

                yield return new WaitForSeconds(2.5f);

                BaloonShaker.Instance.StartWarningShake();

                bangingAudioSource.Play();
                
                player.Die(PlayerDeadType.KillerWind);
            }

            IEnumerator SpawnTentacles()
            {
                yield return new WaitForSeconds(1f);

                Instantiate(tentaclesPrefab);
            }
        }

        
    }
}