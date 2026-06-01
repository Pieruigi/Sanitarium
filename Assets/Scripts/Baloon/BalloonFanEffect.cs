using DG.Tweening;
using UnityEngine;

namespace Baloon
{
    public class BalloonFanEffect : MonoBehaviour
    {
        //[SerializeField]
        //int pathIndex;

        ///// <summary>
        ///// 0: both
        ///// 1: direct
        ///// -1: reversed
        ///// </summary>
        //[SerializeField]
        //int pathDirection = 0;

        [SerializeField]
        AudioSource baseAudioSource, detailAudioSource, fireAudioSource;
        
        bool activated = false;

       
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var r = Random.Range(0f, 1f);
            baseAudioSource.time = baseAudioSource.clip.length * r;
            detailAudioSource.time = detailAudioSource.clip.length * r;
            fireAudioSource.time = fireAudioSource.clip.length * r;

            
        }

        // Update is called once per frame
        void Update()
        {

        }

      
       
        
    }
}