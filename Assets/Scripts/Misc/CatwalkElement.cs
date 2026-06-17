using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class CatwalkElement : MonoBehaviour
    {
        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        List<AudioClip> audioClips;

        private void Awake()
        {
            if(audioSource == null) audioSource  = GetComponent<AudioSource>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
           
            if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Ground")) return;
            
            Debug.Log("TEST - AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA:" + collision.collider.gameObject);
            audioSource.clip = audioClips[Random.Range(0, audioClips.Count)];
            audioSource.Play();

            StartCoroutine(ResetKinematicDelayed());
           
            IEnumerator ResetKinematicDelayed()
            {
                yield return new WaitForSeconds(2f);
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }
}