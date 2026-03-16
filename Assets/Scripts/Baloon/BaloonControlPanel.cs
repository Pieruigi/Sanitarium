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
        public const float DragFov = 40f;

        public static UnityAction OnStarted;
        public static UnityAction OnStopped;

        [SerializeField]
        HoldButton starter;

        [SerializeField]
        HoldSlider throttle;

        [SerializeField]
        HoldButton coldButton, warmButton;

        GameObject player;

        bool started = false;

        
    
        Coroutine startupCoroutine;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");

            ResetAndLockThrottle();
            coldButton.Locked = true;
            warmButton.Locked = true;
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

            throttle.OnDragStarted += HandleOnThrottleDragStarted;
            throttle.OnDragStopped += HandleOnThrottleDragStopped;
        }

        private void OnDisable()
        {
            starter.OnPushed -= HandleStarterOnPushed;
            starter.OnReleased -= HandleStarterOnReleased;

            throttle.OnDragStarted -= HandleOnThrottleDragStarted;
            throttle.OnDragStopped -= HandleOnThrottleDragStopped;
        }

        private void HandleOnThrottleDragStarted()
        {
            FOVController.Instance.SetFOV(DragFov);
        }

        private void HandleOnThrottleDragStopped()
        {
            FOVController.Instance.ResetFOV();
        }

        private void HandleStarterOnPushed()
        {
            if (!started)
            {
                startupCoroutine = StartCoroutine(Startup());  

                IEnumerator Startup()
                {
                    yield return new WaitForSeconds(1f);
                   
                    started = true;

                    throttle.Locked = false;
                    coldButton.Locked = false;
                    warmButton.Locked = false;

                    player.GetComponent<FirstPersonController>().EnterBaloon(GetComponentInParent<BaloonController>().transform);
                    //player.transform.parent = transform.parent;
                    
                    OnStarted?.Invoke();
                }
            }
            else
            {
                if(BasePlatform.CurrentPlatform && throttle.sliderValue == 0)
                {
                    started = false;
                    ResetAndLockThrottle();
                    coldButton.Locked = true;
                    warmButton.Locked = true;
                    player.GetComponent<FirstPersonController>().ExitBaloon();
                    player.transform.parent = null;
                    OnStopped?.Invoke();
                }
                else
                {
                    // Button stuck
                }
                
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