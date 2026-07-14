using UnityEngine;

namespace Baloon
{
    public class OneShotAudio : MonoBehaviour
    {
        [SerializeField]
        float delay = 0;

        bool triggered = false;

        [SerializeField]
        AudioSource audioSource;

        private void Awake()
        {
            if(!audioSource)
                audioSource = GetComponent<AudioSource>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag("Player") || triggered) return;

            triggered = true;

            
            audioSource.PlayDelayed(delay);
            
        }
    }
}