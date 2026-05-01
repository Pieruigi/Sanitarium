using Baloon;
using StarterAssets;
using System;
using UnityEngine;

namespace Baloon
{
    public class CarrionFlySpawner : MonoBehaviour
    {
        [SerializeField]
        GameObject prefab;

        [SerializeField]
        int pathIndex = -1;

        GameObject carrionFly;

        bool spawned = false;

        bool follow = false;

        FirstPersonController player;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = FindFirstObjectByType<FirstPersonController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!follow) return;    

            // Follow the player altitude
            var pos = transform.position;
            pos.y = player.transform.position.y;
            transform.position = pos;
        }

        private void OnEnable()
        {
            BaloonPathManager.OnPathSet += HandleOnPathSet;
            BaloonPathManager.OnPathCleared += HandleOnPathhCleared;
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            BaloonPathManager.OnPathCleared -= HandleOnPathhCleared;
        }

        private void HandleOnPathSet()
        {
            if(spawned) return;

            if(BaloonPathManager.Instance.GetIndex(BaloonPathManager.Instance.CurrentPath) == pathIndex) follow = true;
        }

        private void HandleOnPathhCleared()
        {
            follow = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (spawned || !follow) return;

            spawned = true;

            // Choose the spawn point which is behind the player
            var point = other.transform.position - other.transform.forward * 20f;

            // Spawn the hornet
            carrionFly = Instantiate(prefab);

            carrionFly.transform.position = point;
            
            // Set to attack
            //carrionFly.GetComponent<CarrionFlyController>().StartAttacking();
            
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
        }
    }
}