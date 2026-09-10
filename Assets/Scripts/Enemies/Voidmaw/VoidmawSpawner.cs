using StarterAssets;
using System;
using UnityEngine;

namespace Baloon
{
    public class VoidmawSpawner : MonoBehaviour
    {
        [SerializeField]
        GameObject prefab;

        [SerializeField]
        int pathIndex = -1;

        bool follow = false;
        bool triggered = false;

        FirstPersonController player;

        private void Start()
        {
            player = FindFirstObjectByType<FirstPersonController>();    
        }


        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    Instantiate(prefab);
            //}
#endif

            if (!follow) return;

            // Follow the player altitude
            var pos = transform.position;
            pos.y = player.transform.position.y;
            transform.position = pos;


        }

        private void OnEnable()
        {
            BaloonPathManager.OnPathSet += HandleOnPathSet;
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
        }

        private void HandleOnPathSet()
        {
            if (triggered) return;

            int index = BaloonPathManager.Instance.GetIndex(BaloonPathManager.Instance.CurrentPath);
            if(index == pathIndex)
                follow = true;
        }

        private void HandleOnPathCleared()
        {
            follow = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (!follow || triggered) return;

           
            triggered = true;

            // Instantiate object
            var voidmaw = Instantiate(prefab);

            
        }

      
    }
}