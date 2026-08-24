using DG.Tweening;
using NUnit.Framework;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Baloon
{
    public class BaloonNoGasLeft : MonoBehaviour
    {
        bool processing = false;

        FirstPersonController player;

        List<BasePlatform> platforms;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = FindFirstObjectByType<FirstPersonController>();

            platforms = FindObjectsByType<BasePlatform>(FindObjectsSortMode.None).ToList();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if (processing) return;

            //if(BoilerController.Instance.GasLeft == 0)
            //{
            //    HandleOnNoGasLeft();
            //}
        }


        private void HandleOnNoGasLeft()
        {
            if (!BaloonPathManager.Instance.HasPath())
            {
                // Check if there is a base under the balloon
                var pList = platforms.OrderBy(p => { return Vector3.ProjectOnPlane(BaloonController.Instance.transform.position - p.transform.position, Vector3.up).magnitude; });

                var p = pList.First();

                //Debug.Log($"TEST - P:{p.transform.parent.gameObject}");

                var dist = Vector3.ProjectOnPlane(BaloonController.Instance.transform.position - p.transform.position, Vector3.up).magnitude;

                if (dist < 20 && p.CompareTag("Fuel"))
                    return;

                // Check if you are in a fuel station

            }

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