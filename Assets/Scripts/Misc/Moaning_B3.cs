using System;
using System.Collections;
using UnityEngine;


namespace Baloon
{
    public class Moaning_B3 : MonoBehaviour
    {
        [SerializeField]
        HoldLever valve;

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
            valve.OnReleased += HandleValveOnRelease;   
        }

        private void OnDisable()
        {
            valve.OnReleased -= HandleValveOnRelease;
        }

        private void HandleValveOnRelease()
        {
            if (played) return;
            
            StartCoroutine(DoPlay());

            IEnumerator DoPlay()
            {
                played = true;
                audioSource.Play();
                yield return new WaitForSeconds(1.5f);
                CameraShake.Instance.PlayMoaningShake(16f);
            }
        }
    }
}