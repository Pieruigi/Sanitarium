using Baloon.SaveSystem;
using DG.Tweening;
using NUnit.Framework;
using StarterAssets;
using System;
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

        [SerializeField]
        List<GameObject> tentacleColliders;

        bool triggered = false;

        FirstPersonController player;

        string saveId;

        public class Data
        {
            public bool triggered;
        }
     

        private void Awake()
        {
            foreach (var tentacle in tentacles)
                tentacle.SetActive(false);

            // Set rigidbodies kinematic
            foreach (var rigidbody in rigidbodies)
                rigidbody.isKinematic = true;

            // Disable tentacle colliders
            foreach(var coll in tentacleColliders)
                coll.SetActive(false);

            // Disable death trigger
            deathTrigger.SetActive(false);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var rawData = SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData);
                triggered = data.triggered;

                if (triggered)
                {
                    // Remove rocks
                    foreach (Rigidbody rb in rigidbodies)
                        Destroy(rb.gameObject);
                }
            }

            player = FindFirstObjectByType<FirstPersonController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
        }

        private void OnDisable()
        {
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
        }

        private void HandleOnUpdateDataEntry()
        {
            Data data = new Data();
            data.triggered = triggered;
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
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

                // Enable back tentacles
                tentacles[0].SetActive(true);

                // Enable back collider
                tentacleColliders[0].SetActive(true);

                // Rotate pivot
                var pivot = tentacles[0].transform.parent;
                pivot.DORotate(Vector3.zero, 1f);

                yield return new WaitForSeconds(1.5f);

                float shakeTime = 4f;

                // Do shake
                CameraShake.Instance.PlayRockBridgeCollapseShake(shakeTime);

                yield return new WaitForSeconds(shakeTime);

                var isPlayerOnBridge = IsPlayerOnBridge();

                // Collapse
                foreach(var rigidbody in rigidbodies)
                {
                    rigidbody.GetComponent<MeshCollider>().convex = true;
                    //rigidbody.GetComponent<MeshCollider>().enabled = false;
                    rigidbody.isKinematic = false;
                }

                // Move tentacle down
                foreach(var tentacle in tentacles)
                {
                    // Start release animation
                    tentacle.GetComponentInChildren<Animator>().SetTrigger("Release");

                    // Move down
                    var y = tentacle.transform.position.y - 800f;

                    tentacle.transform.DOMoveY(y, 4f).SetEase(Ease.InSine);
                    
                    Debug.Log("TEST - Move tentacle");
                }

                if (isPlayerOnBridge)
                {
                    // Player is dead
                    player.Doomed = true;

                    // Just wait a little while before player fall
                    var gravity = player.Gravity;
                    Debug.Log("TEST - Gravity:" +  gravity);    
                    player.Gravity = 0;

                    // Disable player movement
                    player.MoveDisabled = true;

                    // Disable collisions
                    player.GetComponent<CharacterController>().detectCollisions = false;

                    yield return new WaitForSeconds(.5f);

                    player.Gravity = -9.8f;
                    Debug.Log("TEST - Player.Gravity:" + player.Gravity);

                    yield return new WaitForSeconds(2f);

                    player.Die(PlayerDeadType.RockBridgeFall);
                    
                }
                else
                {
                    StartCoroutine(DestroyRocksDelayed(10f));
                }
                    
            }

            IEnumerator DestroyRocksDelayed(float delay)
            {
                yield return new WaitForSeconds(delay);
                foreach (var rigidbody in rigidbodies)
                    Destroy(rigidbody.gameObject);
            }
        }

        bool IsPlayerOnBridge()
        {
            var origin = player.transform.position + Vector3.up * .5f;
            RaycastHit hit;

            if(Physics.Raycast(origin, Vector3.down, out hit, 1f))
            {
                var rb = hit.collider.GetComponent<Rigidbody>();
                if (rb == null) return false;

                if (rigidbodies.Contains(rb)) return true;
            }

            return false;
        }
    }
}