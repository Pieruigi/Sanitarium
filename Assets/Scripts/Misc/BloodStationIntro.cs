using System;
using UnityEngine;

namespace Baloon
{
    public class BloodStationIntro : MonoBehaviour
    {
        [SerializeField]
        BlooderController blooder;

        bool inside = false;

        bool done = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!inside || done) return;
        }

        private void OnEnable()
        {
            BaloonControlPanel.OnStopped += HandleOnStopped;
        }

        private void OnDisable()
        {
            BaloonControlPanel.OnStopped -= HandleOnStopped;
        }

        private void HandleOnStopped()
        {
            if (!blooder.Sealed && inside)
            {
                done = true;
                GetComponent<AudioSource>().Play();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            inside = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            inside = false;
        }
    }
}