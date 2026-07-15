using DG.Tweening;
using Mono.Cecil.Cil;
using UnityEngine;

namespace Baloon
{
    public class BaloonCreek : Singleton<BaloonCreek>
    {
        AudioSource source;

        float length;

        float minVolume = .1f;
        float maxVolume = .25f;

        float targetVolume = 0;
        float volumeSpeed = 1f;

        protected override void Awake()
        {
            base.Awake();
            source = GetComponent<AudioSource>();
            length = source.clip.length;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            AdjustVolumeByFactor(0);
        }

        // Update is called once per frame
        void Update()
        {
         
        }

        public void Play(float duration)
        {
            return;
            if (source.isPlaying) source.Pause();

            float min = 0f;
            float max = length - duration;

            source.time = Random.Range(min, max);

            source.Play();
        }

        public void Play() 
        {
            source.Play();
        }

        public void Stop()
        {
            source.Pause();
        }

        public void AdjustVolumeByFactor(float factor)
        {
            var v = Mathf.Lerp(minVolume, maxVolume, factor);
            var t = Mathf.Abs(source.volume - v) / volumeSpeed;
            source.DOFade(v, t);
        }
    }
}