using System;
using System.Collections;
using UnityEngine;

namespace Baloon
{
    public class CreepyChonchon : MonoBehaviour
    {
        [SerializeField]
        GameObject mesh;

        [SerializeField]
        AudioSource wingsAudioSource;

        int path = 5;

        bool running = false;

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
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
        }

        private void HandleOnPathSet()
        {
            if (running) return;

            if(BaloonPathManager.Instance.TryGetCurrentPathIndex(out var p))
            {
                if(p == path)
                {
                    StartCoroutine(DoPlay());
                }
            }

            IEnumerator DoPlay()
            {
                yield return new WaitForSeconds(3f);

                if (running) yield break;

                // Activate
                running = true;


            }
        }
    }
}