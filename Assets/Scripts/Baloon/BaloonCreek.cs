using DG.Tweening;
using Mono.Cecil.Cil;
using System.Collections;
using System.Numerics;
using UnityEngine;

namespace Baloon
{
    public class BaloonCreek : Singleton<BaloonCreek>
    {
        AudioSource source;

        float length;

        float minVolume = .4f;
        float maxVolume = .6f;

        
        protected override void Awake()
        {
            base.Awake();
            source = GetComponent<AudioSource>();
            length = source.clip.length;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
         
        }

        public void Play(float duration, float power)
        {
            if (source.isPlaying) source.Stop();

            float min = 0f;
            float max = length - duration;

            source.time = Random.Range(min, max);

            //AdjustVolumeByFactor(power);
            source.volume = Mathf.Lerp(minVolume, maxVolume, Mathf.Clamp01(power));
            
            source.Play();
            
            StartCoroutine(DoStop(duration));

            IEnumerator DoStop(float time)
            {
                yield return new WaitForSeconds(time);
                source.Stop();
            }
        }

     
        
    }
}