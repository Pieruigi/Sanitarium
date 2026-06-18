using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class LoreMoan_259_406 : MonoBehaviour
    {
        [SerializeField]
        List<AudioClip> clips;

        AudioSource audioSource;

        bool playing, playingLast;

        private void Awake()
        {
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

        private void LateUpdate()
        {
            

            float dist = Vector3.Distance(Camera.main.transform.position, transform.position);
            if(dist < audioSource.maxDistance)
            {
                if(!audioSource.isPlaying)
                {
                    // Choose a random clip
                    audioSource.clip = clips[Random.Range(0, clips.Count)]; 
                    audioSource.Play();
                }    
            }
            
        }
    }
}