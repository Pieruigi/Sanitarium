using System;
using UnityEngine;

namespace Baloon
{

    public class LookingEyesYell : MonoBehaviour
    {
        [SerializeField]
        HoldLever lever;

        AudioSource audioSource;

        bool played = false;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
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
            lever.OnReleased += HandleOnLeverReleased;
        }

        private void OnDisable()
        {
            lever.OnReleased -= HandleOnLeverReleased;
        }

        private void HandleOnLeverReleased()
        {
            if (played) return;

            played = true;

            audioSource.Play();
        }
    }
}