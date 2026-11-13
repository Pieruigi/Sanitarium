using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Baloon
{
    public class DoorBanging : MonoBehaviour
    {
        [SerializeField]
        HoldLever button;

        [SerializeField]
        Collider _collider;

        [SerializeField]
        AudioSource doorAudioSource;

        bool triggered = false;

        private void Awake()
        {
            _collider.enabled = false;
        }


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
            button.OnReleased += HandleOnReleased;
        }

        private void OnDisable()
        {
            button.OnReleased -= HandleOnReleased;
        }

        private void HandleOnReleased()
        {
            _collider.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || triggered) return;

            triggered = true;

            StartCoroutine(DoScare());

            IEnumerator DoScare()
            {
                doorAudioSource.Play();

                yield return new WaitForSeconds(.5f);

                CameraShake.Instance.PlayJumpscare(1);
            }
        }
    }
}