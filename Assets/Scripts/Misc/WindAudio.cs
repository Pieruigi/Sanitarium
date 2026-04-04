using DG.Tweening;
using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace Baloon
{
    public class WindAudio : Singleton<WindAudio>
    {

        [SerializeField]
        AudioSource windAudioSource;

        float volumeDefault;
        float lightVolume, heavyVolume, gustVolume, killerVolume;

        float pitchDefault;
        float lightPitch, heavyPitch, gustPitch, killerPitch;

        Sequence volumeSequence;

        protected override void Awake()
        {
            base.Awake();
            volumeDefault = windAudioSource.volume;
            lightVolume = volumeDefault * 2;
            heavyVolume = volumeDefault * 3;
            gustVolume = volumeDefault * 4;
            killerVolume = volumeDefault * 6;


            pitchDefault = windAudioSource.pitch;
            lightPitch = pitchDefault * 1.2f;
            heavyPitch = pitchDefault * 1.4f;
            gustPitch = pitchDefault * 1.5f;
            killerPitch = pitchDefault * 1.65f;
        }

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
            PlayWind();
        }

        private void HandleOnPathCleared()
        {
            StopWind();
        }

        void PlayWind()
        {
            windAudioSource.volume = 0;
            windAudioSource.Play();
            windAudioSource.DOFade(volumeDefault, 2f);
        }

        void StopWind()
        {
            windAudioSource.DOFade(0f, 2f).OnComplete(() => { windAudioSource.volume = 0; windAudioSource.Stop(); });
        }

        
        public void FadeLightVolume(float duration)
        {
            if (volumeSequence != null) volumeSequence.Kill();

            volumeSequence = DOTween.Sequence();
            volumeSequence.Append(windAudioSource.DOFade(lightVolume, .5f));
            volumeSequence.Join(windAudioSource.DOPitch(lightPitch, .5f));
            volumeSequence.AppendInterval(duration);
            volumeSequence.Append(windAudioSource.DOFade(volumeDefault, .5f));
            volumeSequence.Join(windAudioSource.DOPitch(pitchDefault, .5f));
        }

        public void FadeHeavyVolume(float duration)
        {
            if (volumeSequence != null) volumeSequence.Kill();

            volumeSequence = DOTween.Sequence();
            volumeSequence.Append(windAudioSource.DOFade(heavyVolume, .5f));
            volumeSequence.Join(windAudioSource.DOPitch(heavyPitch, .5f));
            volumeSequence.AppendInterval(duration);
            volumeSequence.Append(windAudioSource.DOFade(volumeDefault, .5f));
            volumeSequence.Join(windAudioSource.DOPitch(pitchDefault, .5f));
        }

        public void FadeGustVolume(float duration)
        {
            if (volumeSequence != null) volumeSequence.Kill();

            volumeSequence = DOTween.Sequence();
            volumeSequence.Append(windAudioSource.DOFade(gustVolume, .5f));
            volumeSequence.Join(windAudioSource.DOPitch(gustPitch, .5f));
            volumeSequence.AppendInterval(duration);
            volumeSequence.Append(windAudioSource.DOFade(volumeDefault, .5f));
            volumeSequence.Join(windAudioSource.DOPitch(pitchDefault, .5f));
        }

        public void FadeKillerVolume(float duration)
        {
            if (volumeSequence != null) volumeSequence.Kill();

            if (!windAudioSource.isPlaying) windAudioSource.Play();

            volumeSequence = DOTween.Sequence();
            volumeSequence.Append(windAudioSource.DOFade(killerVolume, duration));
            volumeSequence.Join(windAudioSource.DOPitch(killerPitch, duration));
            //volumeSequence.AppendInterval(duration);
            //volumeSequence.Append(windAudioSource.DOFade(volumeDefault, .5f));
            //volumeSequence.Join(windAudioSource.DOPitch(pitchDefault, .5f));
        }

        public void FadeReset()
        {
            if (volumeSequence != null) volumeSequence.Kill();
            volumeSequence.Append(windAudioSource.DOFade(volumeDefault, .5f));
            volumeSequence.Join(windAudioSource.DOPitch(pitchDefault, .5f));
        }
    }
}