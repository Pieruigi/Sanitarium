using System;
using UnityEngine;
using UnityEngine.Events;

namespace Baloon
{
    public class FuelTower : MonoBehaviour
    {
        public UnityAction OnEnter;
        public UnityAction OnExit;

        [SerializeField]
        GameObject plug;

        [SerializeField]
        GameObject pipeGroup;

        [SerializeField]
        AudioSource pipesAudioSource;

        bool inside = false;
        
        float shakeTime = .5f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            plug.SetActive(true);
            pipeGroup.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            BaloonControlPanel.OnStarted += HandleOnStarted;
            BaloonControlPanel.OnStopped += HandleOnStopped;
        }

        private void OnDisable()
        {
            BaloonControlPanel.OnStarted -= HandleOnStarted;
            BaloonControlPanel.OnStopped -= HandleOnStopped;
        }

        private void HandleOnStarted()
        {
            if (!inside) return;

            plug.SetActive(true);
            pipeGroup.SetActive(false);
            CameraShake.Instance.PlayJumpscare(shakeTime);
            pipesAudioSource.Play();
        }

        private void HandleOnStopped()
        {
            if (!inside) return;

            plug.SetActive(false);
            pipeGroup.SetActive(true);
            CameraShake.Instance.PlayJumpscare(shakeTime);
            pipesAudioSource.Play();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Baloon")) return;

            inside = true;

            // Lets be sure the balloon is running
            var panel = FindFirstObjectByType<BaloonControlPanel>();
            if(panel != null)
            {
                if(!panel.IsRunning)
                {
                    plug.SetActive(false);
                    pipeGroup.SetActive(true);
                    CameraShake.Instance.PlayJumpscare(shakeTime);
                    pipesAudioSource.Play();
                }
            }

            OnEnter?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Baloon")) return;

            inside = false;

            OnExit?.Invoke();
        }

        
    }
}