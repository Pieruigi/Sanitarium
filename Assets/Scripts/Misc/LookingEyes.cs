using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Baloon
{
    public class LookingEyes : MonoBehaviour
    {
        [SerializeField]
        HoldLever lever;

        [SerializeField]
        GameObject eyes;

        bool isActive = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            eyes.SetActive(isActive);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if (!isActive) return;

            transform.LookAt(Camera.main.transform.position);  

        }

        private void OnEnable()
        {
            lever.OnPushed += HandleOnPushed;
            BaloonControlPanel.OnStarted += HandleOnStarted;
        }

        private void OnDisable()
        {
            lever.OnPushed -= HandleOnPushed;
            BaloonControlPanel.OnStarted -= HandleOnStarted;
        }

        private void HandleOnPushed()
        {
            Debug.Log("TEST - AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            isActive = true;
            if(!eyes.activeSelf)
                eyes.SetActive(true);
        }

        private void HandleOnStarted()
        {
            isActive = false;
            if (eyes.activeSelf)
                eyes.SetActive(false);
        }
    }
}