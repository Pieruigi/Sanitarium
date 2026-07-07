using DG.Tweening;
using StarterAssets;
using System;
using System.Collections;
using UnityEngine;

namespace Baloon
{
    public class BaloonNoGasLeft : MonoBehaviour
    {
        bool processing = false;

        FirstPersonController player;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = FindFirstObjectByType<FirstPersonController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if (processing) return;

            if(BoilerController.Instance.GasLeft == 0)
            {
                HandleOnNoGasLeft();
            }
        }


        private void HandleOnNoGasLeft()
        {
            processing = true;

            
            DoProcess();

            void DoProcess()
            {
               
                // Play camera shake
                //CameraShake.Instance.PlayWindGustShake(duration);
                KillerWind.Instance.PlayNoGasKill();
                


            }

           

            

        }
    }
}