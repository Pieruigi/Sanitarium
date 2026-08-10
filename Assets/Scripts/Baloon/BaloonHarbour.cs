using System;
using TMM;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Baloon
{
    public class BaloonHarbour : MonoBehaviour
    {
        
        public static bool NotSafe = false;

        private void Awake()
        {
            NotSafe = false; // We always starts from a safe spot
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Baloon")) return;

            // Check if it's a station
            var parent = transform.parent.parent;

            NotSafe = parent.GetComponentInChildren<BasePlatform>() == null;

            
        }


        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Baloon")) return;

            NotSafe = true;
           
        }
        
    }
}