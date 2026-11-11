using Steamworks;
using UnityEngine;

namespace Baloon
{
    public class BaloonSpeaker : MonoBehaviour
    {
        AudioSource audioSource;

        [SerializeField]
        AudioSource noiseAudioSource;

        float volumeDefault;

        float noiseVolumeDefault;

        public float VolumeDefault => volumeDefault;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            volumeDefault = audioSource.volume;
            noiseVolumeDefault = noiseAudioSource.volume;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void LateUpdate()
        {
            if (!audioSource.isPlaying && noiseAudioSource.isPlaying)
                noiseAudioSource.Stop();
        }

        public void Play(AudioClip clip, float volume)
        {
            if (audioSource.isPlaying) audioSource.Stop();

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.PlayDelayed(.2f);

            // Play noise
            noiseAudioSource.volume = noiseVolumeDefault * volume / volumeDefault;
            noiseAudioSource.Play();
        }

        public void Play(AudioClip clip)
        {
            Play(clip, volumeDefault);
        }

        public void Stop()
        {
            audioSource.Stop();
            noiseAudioSource.Stop();
        }

        public bool IsPlaying()
        {
            return audioSource.isPlaying;
        }
    }
}