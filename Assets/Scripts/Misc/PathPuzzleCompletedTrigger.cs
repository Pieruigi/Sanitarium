using Baloon.SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{


    public class PathPuzzleCompletedTrigger : MonoBehaviour
    {

        [SerializeField]
        GameObject blockCollider;

        [SerializeField]
        BlooderController blooder;

        [SerializeField]
        List<GameObject> tiles;

        [SerializeField]
        AudioSource explosionAudioSource;

        class Data
        {
            public bool triggered = false;
        }

        string saveId = "path_z5";

        bool triggered = false;

    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // Save data
            string rawData = SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData);
                triggered = data.triggered;

                if (triggered)
                {
                    // Remove all tiles
                    DisableTiles();
                    
                    
                }
            }

            // Set collider
            blockCollider.SetActive(triggered);
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
            var data = new Data();
            data.triggered = triggered;
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || !blooder.Sealed || triggered) return;

            triggered = true;

            StartCoroutine(DoFall());

            IEnumerator DoFall()
            {
                CameraShake.Instance.PlayJumpscare(1f);

                explosionAudioSource.Play();

                DisableTiles();

                blockCollider.SetActive(true);

                yield break;
            }
        }

        void DisableTiles()
        {
            foreach (var tile in tiles)
            {
                tile.SetActive(false);
            }
        }
    }
}