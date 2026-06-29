using DG.Tweening;
using StarterAssets;
using System;
using System.Collections;
using Unity.XR.OpenVR;
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

        float fogDensityDefault, fogAttenuationDefault;
        
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

            fogDensityDefault = fog.density.value;
            fogAttenuationDefault = fog.attenuationDistance.value;
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR


            //if (Input.GetKeyDown(KeyCode.C))
            //    StartKilling();
#endif
        }

        //private void LateUpdate()
        //{
            
        //    if (!running || killing) return;

        //    // Get the altitude range
        //    var range = AltitudeManager.Instance.GetCurrentRange();

        //    // Check
        //    switch (range)
        //    {
        //        case AltitudeRange.Green:
        //        case AltitudeRange.Yellow:
        //            if(!killing)
        //                killElapsed = 0;
        //            //StopWarning();
        //            break;
        //        case AltitudeRange.Red:
        //            killElapsed += Time.deltaTime;

        //            if (killElapsed > killTime)
        //            {
        //                // You die
        //                StartKilling();
        //            }
        //            break;
        //    }

           
        //}

        //private void OnEnable()
        //{
        //    BasePlatform.OnLanding += HandleOnLanding;
        //    BasePlatform.OnTakeOff += HandleOnTakeOff;
        //    BaloonPathManager.OnPathSet += HandleOnPathSet;
        //    BaloonPathManager.OnPathCleared += HandleOnPathCleared;
        //}

        //private void OnDisable()
        //{
        //    BasePlatform.OnLanding -= HandleOnLanding;
        //    BasePlatform.OnTakeOff -= HandleOnTakeOff;
        //    BaloonPathManager.OnPathSet -= HandleOnPathSet;
        //    BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
        //}

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

        public void StartKilling()
        {
            if (killing || player.Doomed) return;
            killing = true;
            player.Doomed = true;

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

        public void PlayNoGasKill()
        {
            if (killing || player.Doomed) return;
            killing = true;
            player.Doomed = true;

            StartCoroutine(DoKill());

            OnKilling?.Invoke();

            IEnumerator DoKill()
            {
                // Stop the wind shaker
                WindShaker.Instance.Running = false;

                // Stop the constante vertical wind
                VerticalWind.Instance.Running = false;

                hauntingAudioSource.Play();

                DOTween.To(() => fog.density.value, x => fog.density.value = x, .6f, 10.5f);
                DOTween.To(() => fog.attenuationDistance.value, x => fog.attenuationDistance.value = x, 5, 10.5f);

                yield return new WaitForSeconds(1.5f);

                // Move the balloon up 
                var targetY = transform.position.y + 40;

                var duration = 3f;

               

                // 2. Move the balloon violently (Ease.OutQuad starts fast and then slows down)
                BaloonController.Instance.transform.DOMoveY(targetY, duration).SetEase(Ease.OutQuad);

                BaloonController.Instance.DisableVerticalVelocity();

                BaloonShaker.Instance.StartWarningShake(duration);
                CameraShake.Instance.PlayKillerWindShake(duration);

                // Apply audio
                //WindAudio.Instance.FadeGustVolume(duration);
                WindAudio.Instance.FadeKillerVolume(duration);

                bangingAudioSource.Play();
                breakingAudioSource.PlayDelayed(2.5f);

                player.Die(PlayerDeadType.KillerWind);
            }

           
        }

        Sequence warningSeq;

        public void StartWarning(float time)
        {
            if (warningSeq != null) warningSeq.Kill();
            
            hauntingAudioSource.Play();
            warningSeq = DOTween.Sequence();
            warningSeq.Append(DOTween.To(() => fog.density.value, x => fog.density.value = x, .6f, time));
            warningSeq.Join(DOTween.To(() => fog.attenuationDistance.value, x => fog.attenuationDistance.value = x, 5, time));
            
        }

        public void StopWarning()
        {
            hauntingAudioSource.Stop();
            if (warningSeq == null) return;
            warningSeq.Kill();

            warningSeq = DOTween.Sequence();
            warningSeq.Append(DOTween.To(() => fog.density.value, x => fog.density.value = x, fogDensityDefault, 1f));
            warningSeq.Join(DOTween.To(() => fog.attenuationDistance.value, x => fog.attenuationDistance.value = x, fogAttenuationDefault, 1f));
        }

        
    }
}