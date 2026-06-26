using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class ChonchonWingAudio : MonoBehaviour
    {
        [SerializeField]
        AudioSource flapAudioSource;

        [SerializeField]
        List<AudioClip> flapAudioClips;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PlayWingFlap()
        {
            flapAudioSource.clip = flapAudioClips[Random.Range(0, flapAudioClips.Count)];
            flapAudioSource.Play();
        }
    }
}