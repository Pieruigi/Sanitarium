using NUnit.Framework;
using StarterAssets;
using System;
using System.Collections;
using System.Linq;
using TMM;
using Unity.VisualScripting;
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

        //[SerializeField]
        //HoldButton coldButton, warmButton;

        [SerializeField]
        AudioSource startAudioSource, runAudioSource, stopAudioSource;

        float runAudioPitchMin = 1f, runAudioPitchMax = 1.4f;
      
        GameObject player;

        bool started = false;
        public bool IsRunning { get { return started; } }

        bool releasePlayerOnLanding = false;
        
    
        Coroutine startupCoroutine;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");

            ResetAndLockThrottle();
            //coldButton.Locked = true;
            //warmButton.Locked = true;
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //    FlyBack();
#endif
            if(BoilerController.Instance.GasLeft == 0)
            {
                if(started)
                {
                    // Stop running audio
                    runAudioSource.Stop();
                    // Play stopping audio
                    stopAudioSource.Play();

                    started = false;
                    

                    releasePlayerOnLanding = true;

                    OnStopped?.Invoke();
                }
                
            }
            
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
            throttle.OnValueChanged += HandleOnThrottleValueChanged;

            BasePlatform.OnLanding += HandleOnLanding;
        }

        private void OnDisable()
        {
            starter.OnPushed -= HandleStarterOnPushed;
            starter.OnReleased -= HandleStarterOnReleased;

            throttle.OnDragStarted -= HandleOnThrottleDragStarted;
            throttle.OnDragStopped -= HandleOnThrottleDragStopped;
            throttle.OnValueChanged -= HandleOnThrottleValueChanged;

            BasePlatform.OnLanding -= HandleOnLanding;
        }

        private void HandleOnLanding(BasePlatform platform)
        {
            
            if (releasePlayerOnLanding)
            {
                releasePlayerOnLanding = false;
                player.GetComponent<FirstPersonController>().ExitBaloon();
                ResetAndLockThrottle();
                //player.transform.parent = null;
            }
        }

        private void HandleOnThrottleValueChanged(float value)
        {
            if (!started) return;

            runAudioSource.pitch = Mathf.Lerp(runAudioPitchMin, runAudioPitchMax, value);
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
                if(BoilerController.Instance.GasLeft > 0)
                {
                    startupCoroutine = StartCoroutine(Startup());

                    IEnumerator Startup()
                    {
                        // Play starting audio
                        startAudioSource.Play();

                        yield return new WaitForSeconds(1f);

                        started = true;

                        throttle.Locked = false;
                        //coldButton.Locked = false;
                        //warmButton.Locked = false;

                        player.GetComponent<FirstPersonController>().EnterBaloon(GetComponentInParent<BaloonController>().transform);
                        //player.transform.parent = transform.parent;

                        // Play running audio
                        runAudioSource.Play();

                        OnStarted?.Invoke();
                    }
                }
                else // No gas
                {
                    startAudioSource.Play();
                }
               
            }
            else
            {
                if(BasePlatform.CurrentPlatform && throttle.sliderValue == 0)
                {
                    // Stop running audio
                    runAudioSource.Stop();
                    // Play stopping audio
                    stopAudioSource.Play();

                    started = false;
                    ResetAndLockThrottle();
                    //coldButton.Locked = true;
                    //warmButton.Locked = true;
                    player.GetComponent<FirstPersonController>().ExitBaloon();
                    //player.transform.parent = null;
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
                // Stop starting audio
                //startAudioSource.Stop();

                // Stop starting coroutine
                StopCoroutine(startupCoroutine);
            }
            
        }

        void ResetAndLockThrottle()
        {
            throttle.ResetSlider();
            throttle.Locked = true;
        }

        void FlyBack()
        {
            BaloonPathManager.Instance.ReversePath();
        }

        public void DisableControls()
        {
            throttle.GetComponent<Interactor>().enabled = false;
            starter.GetComponent<Interactor>().enabled = false;
        }

        public void EnableControls()
        {
            throttle.GetComponent<Interactor>().enabled = true;
            starter.GetComponent<Interactor>().enabled = true;
        }
    }
}