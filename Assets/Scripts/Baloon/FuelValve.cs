using System;
using UnityEngine;

namespace Baloon
{
    public class FuelValve : MonoBehaviour
    {
        [SerializeField]
        HoldLever lever;

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
            lever.OnPushed += HandleOnPushed;
            lever.OnReleased += HandleOnRelease;
        }

        private void OnDisable()
        {
            lever.OnPushed -= HandleOnPushed;
            lever.OnReleased -= HandleOnRelease;
        }

        private void HandleOnPushed()
        {
            
        }

        private void HandleOnRelease()
        {
            
        }
    }
}