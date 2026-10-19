using DG.Tweening;
using NUnit.Framework;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{

    public class BridgeController : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> tentacles;

        [SerializeField]
        List<Rigidbody> rigidbodies;

        [SerializeField]
        GameObject deathTrigger;

        bool triggered = false;

        FirstPersonController player;

        private void Awake()
        {
            foreach (var tentacle in tentacles)
                tentacle.SetActive(false);

            // Set rigidbodies kinematic
            foreach (var rigidbody in rigidbodies)
                rigidbody.isKinematic = true;

            // Disable death trigger
            deathTrigger.SetActive(false);
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

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || triggered) return;

            triggered = true;

            StartCoroutine(DoPlay());

            IEnumerator DoPlay()
            {
                // Enable death trigger
                deathTrigger.SetActive(true);

                // Enable tentacles
                tentacles[0].SetActive(true);

                // Rotate pivot
                var pivot = tentacles[0].transform.parent;
                pivot.DORotate(Vector3.zero, 1f);

                yield return new WaitForSeconds(1.5f);

                float shakeTime = 8f;

                // Do shake
                CameraShake.Instance.PlayRockBridgeCollapseShake(shakeTime);

                yield return new WaitForSeconds(shakeTime);

                // Collapse
                foreach(var rigidbody in rigidbodies)
                {
                    rigidbody.GetComponent<MeshCollider>().convex = true;
                    //rigidbody.GetComponent<MeshCollider>().enabled = false;
                    rigidbody.isKinematic = false;
                }

                // Just wait a little while before player fall
                var gravity = player.Gravity;
                player.Gravity = 0;

                yield return new WaitForSeconds(1f);

                player.Gravity = gravity;

                // Disable player movement
                player.MoveDisabled = true; 
                    
            }
        }
    }
}