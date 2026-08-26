using Steamworks;
using UnityEngine;

namespace Baloon
{
    public class BaloonSpeaker : MonoBehaviour
    {
        AudioSource audioSource;

        float volumeDefault = 1.0f;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            volumeDefault = audioSource.volume;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Play(AudioClip clip, float volume)
        {
            if (audioSource.isPlaying) audioSource.Stop();

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
        }

        public void Play(AudioClip clip)
        {
            Play(clip, volumeDefault);
        }

        public void Stop()
        {
            audioSource.Stop();
        }

        public bool IsPlaying()
        {
            return audioSource.isPlaying;
        }
    }
}