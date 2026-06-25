using System;
using UnityEngine;

namespace Baloon
{
    public class ChonchonIdleTrigger : MonoBehaviour
    {
        ChonchonController chonchon;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            ChonchonController.OnSpawned += HandleOnSpawned;
        }

        private void OnDisable()
        {
            ChonchonController.OnSpawned -= HandleOnSpawned;
        }

        private void HandleOnSpawned(ChonchonController chonchon)
        {
            this.chonchon = chonchon;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            chonchon.SetIdleState();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            var dir = Vector3.ProjectOnPlane(other.transform.position - transform.position, Vector3.up);
            if (Vector3.Dot(dir, transform.forward) > 0)
                chonchon.UnsetIdleState();
        }
    }
}