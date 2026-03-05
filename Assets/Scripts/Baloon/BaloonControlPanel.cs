using StarterAssets;
using System;
using System.Collections;
using TMM;
using UnityEngine;
using UnityEngine.Events;

namespace Baloon
{
    public class BaloonControlPanel : MonoBehaviour
    {

        public static UnityAction OnStarted;
        public static UnityAction OnStopped;

        [SerializeField]
        HoldButton starter;

        [SerializeField]
        HoldSlider throttle;

        GameObject player;

        bool started = false;

    
        Coroutine startupCoroutine;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");

            ResetAndLockThrottle();
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
           
#endif


        }

        private void LateUpdate()
        {
            // Always rotate to face the player
            var dir = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up);
            transform.forward = dir.normalized;
        }

        private void OnEnable()
        {
            starter.OnPushed += HandleStarterOnPushed;
            starter.OnReleased += HandleStarterOnReleased;
        }

        private void OnDisable()
        {
            starter.OnPushed -= HandleStarterOnPushed;
            starter.OnReleased -= HandleStarterOnReleased;
        }


        private void HandleStarterOnPushed()
        {
            if (!started)
            {
                startupCoroutine = StartCoroutine(Startup());  

                IEnumerator Startup()
                {
                    yield return new WaitForSeconds(3f);
                   
                    started = true;

                    throttle.Locked = false;

                    player.GetComponent<FirstPersonController>().OnBaloon = true;
                    player.transform.parent = transform;

                    OnStarted?.Invoke();
                }
            }
            else
            {
                started = false;
                ResetAndLockThrottle();
                player.GetComponent<FirstPersonController>().OnBaloon = false;
                player.transform.parent = null;
                OnStopped?.Invoke();
            }

        }

        private void HandleStarterOnReleased()
        {
            if (!started)
            {
                StopCoroutine(startupCoroutine);
            }
            
        }

        void ResetAndLockThrottle()
        {
            throttle.ResetSlider();
            throttle.Locked = true;
        }
    }
}