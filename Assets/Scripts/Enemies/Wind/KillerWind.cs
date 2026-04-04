using DG.Tweening;
using StarterAssets;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Baloon
{
    public class KillerWind : Singleton<KillerWind>
    {
        public static UnityAction OnKilling;
        
        

        [SerializeField]
        float safeZoneKillTime; // How much time in red range before wind kills you

        [SerializeField]
        float travelZoneKillTime;

        [SerializeField]
        AudioSource hauntingAudioSource;

        [SerializeField]
        AudioSource bangingAudioSource;

        [SerializeField]
        AudioSource breakingAudioSource;

        [SerializeField]
        GameObject tentaclesPrefab;

        [SerializeField]
        Volume globalVolume;

        VolumetricFogVolumeComponent fog;

        
        float killElapsed = 0;

        float killTime;

        bool running = false;

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

            globalVolume.profile.TryGet<VolumetricFogVolumeComponent>(out fog);
            
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
                    if(!killing)
                        killElapsed = 0;
                    //StopWarning();
                    break;
                case AltitudeRange.Red:
                    killElapsed += Time.deltaTime;

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
          
        }

        private void HandleOnPathCleared()
        {
            killTime = safeZoneKillTime;
        
        }

        private void HandleOnLanding(BasePlatform platform)
        {
            running = false;
            killTime = safeZoneKillTime;
            killElapsed = 0;
        }

        private void HandleOnTakeOff(BasePlatform platform)
        {
            running = true;
            killTime = safeZoneKillTime;
            killElapsed = 0;
        }

        void StartKilling()
        {
            if (killing) return;
            killing = true;

            //StartCoroutine(SpawnTentacles());
            StartCoroutine(DoKill());

            OnKilling?.Invoke();
            
            
            IEnumerator DoKill()
            {
                WindAudio.Instance.FadeReset();

                hauntingAudioSource.Play();

                DOTween.To(() => fog.density.value, x => fog.density.value = x, .6f, 2f);
                DOTween.To(() => fog.attenuationDistance.value, x => fog.attenuationDistance.value = x, 5, 2f);

                yield return new WaitForSeconds(1f);

                Instantiate(tentaclesPrefab);

                yield return new WaitForSeconds(1.5f);

                BaloonShaker.Instance.StartWarningShake(4f);
                CameraShake.Instance.PlayKillerWindShake(4f);

                bangingAudioSource.Play();
                breakingAudioSource.PlayDelayed(2.5f);
                
                player.Die(PlayerDeadType.KillerWind);
            }

        }

        
    }
}