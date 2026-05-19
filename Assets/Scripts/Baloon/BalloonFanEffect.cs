using DG.Tweening;
using UnityEngine;

namespace Baloon
{
    public class BalloonFanEffect : MonoBehaviour
    {
        [SerializeField]
        int pathIndex;

        /// <summary>
        /// 0: both
        /// 1: direct
        /// -1: reversed
        /// </summary>
        [SerializeField]
        int pathDirection = 0;

        [SerializeField]
        AudioSource baseAudioSource, detailAudioSource, fireAudioSource;
        
        bool activated = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Debug.Log($"TEST - BalloonEffect Start");

            var r = Random.Range(0f, 1f);
            baseAudioSource.time = baseAudioSource.clip.length * r;
            detailAudioSource.time = detailAudioSource.clip.length * r;
            fireAudioSource.time = fireAudioSource.clip.length * r;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            //BaloonPathManager.OnPathSet += HandleOnPathSet;
            //BaloonPathManager.OnPathCleared += HandleOnPathCleared;
            //HandleOnPathSet();
        }

        private void OnDisable()
        {
            //BaloonPathManager.OnPathSet -= HandleOnPathSet;
            //BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
            //HandleOnPathCleared();
        }

        private void HandleOnPathSet()
        { 
            Debug.Log($"TEST - BalloonEffect path set");
            if (BaloonPathManager.Instance.TryGetCurrentPathIndex(out var index))
            {
                
                var isReversed = BaloonPathManager.Instance.IsPathReversed;

                Debug.Log($"TEST - Index:{index}, Reversed:{isReversed}");

                if (index == pathIndex && (pathDirection == 0 || (pathDirection == 1 && !isReversed) || (pathDirection == -1 && isReversed)))
                    activated = true;

                if (activated)
                {
                    ActivateFX();
                }
            }

            
        }

        private void HandleOnPathCleared()
        {
            if (activated)
            {
                activated = false;
                DeactivateFX();
            }
        }

        void ActivateFX()
        {
            var r = Random.Range(0f, 1f);
            baseAudioSource.time = baseAudioSource.clip.length * r;
            detailAudioSource.time = detailAudioSource.clip.length * r;

            baseAudioSource.Play();
            detailAudioSource.Play();
        }

        void DeactivateFX()
        {
            baseAudioSource.Stop();
            detailAudioSource.Stop();
        }
    }
}