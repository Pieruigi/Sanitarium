using System;
using System.Collections;
using TMM;
using UnityEngine;

namespace Baloon
{


    public class BaloonLauncherController : MonoBehaviour
    {
        [SerializeField]
        BaloonLauncher launcher;

        [SerializeField]
        ActivationTrigger activator;

        
        [SerializeField]
        AudioSource mechanicalAudioSource;

        int direction = -1;

        bool inside = false;
        public bool Inside => inside;

        bool launching = false;

        bool launched = false;

        BaloonControlPanel controlPanel;

        BaloonLauncherButtonFx buttonFx;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            direction = launcher.GetFirstAvailableDirection(); // Launcher has just on direction in the new gameplay
            controlPanel = FindFirstObjectByType<BaloonControlPanel>();
            buttonFx = FindFirstObjectByType<BaloonLauncherButtonFx>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!inside || !controlPanel.IsRunning || launched || launcher.IsDisabled) return;

            
            var altitudeRange = AltitudeManager.Instance.GetCurrentRange();

            switch (altitudeRange)
            {
                case AltitudeRange.Green:
                    if(!launching)
                    {
                        launching = true;
                    
                        buttonFx.Play();
                    }
                    break;
                default:
                    if (launching)
                    {
                        launching = false;
                        buttonFx.Stop();
                    }
                    break;
            }
            
          

        }


        private void OnEnable()
        {
            activator.OnEnter += HandleOnEnter;
            activator.OnExit += HandleOnExit;
        }

        private void OnDisable()
        {
            activator.OnEnter -= HandleOnEnter;
            activator.OnExit -= HandleOnExit;
        }

        private void HandleOnEnter(Collider other)
        {
            inside = true;
        }

        private void HandleOnExit(Collider other)
        {
            inside = false;
            launching = false;
           
        }

        public void Launch()
        {
            if (!inside || launcher.IsDisabled || BaloonPathManager.Instance.HasPath() || !launching || launched) return;

            
            

            StartCoroutine(DoLauch());

            IEnumerator DoLauch()
            {
                //HandleOnPathSet();
                launched = true;
                
                mechanicalAudioSource.Play();

                launcher.SwitchDirection(launcher.GetFirstAvailableDirection());

                yield return new WaitForSeconds(.125f);

                launcher.SetPathFromCurrentDirection();

                buttonFx.Stop();
            }
        }
    }
}