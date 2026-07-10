using NUnit.Framework;
using StarterAssets;
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

        Rigidbody rb;

        FirstPersonController player;

        private void Awake()
        {
            if(audioSource == null) audioSource  = GetComponent<AudioSource>();

            rb = GetComponent<Rigidbody>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = FindFirstObjectByType<FirstPersonController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            if (rb.isKinematic || player.Doomed) return;

            
            if(Physics.Raycast(Camera.main.transform.position, Vector3.down, out var hitInfo, 2f))
            {
                if(hitInfo.collider.gameObject == gameObject)
                {
                    player.Doomed = true;
                    player.Die(PlayerDeadType.CatwalkCollapsing);
                }
            }

        }

        private void OnCollisionEnter(Collision collision)
        {
           
            if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Ground")) return;
            
           
            audioSource.clip = audioClips[Random.Range(0, audioClips.Count)];
            audioSource.Play();

            StartCoroutine(ResetKinematicDelayed());
           
            IEnumerator ResetKinematicDelayed()
            {
                yield return new WaitForSeconds(4f);
                rb.isKinematic = true;
            }
        }
    }
}