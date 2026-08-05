using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Baloon
{
    public class BaloonDoor : MonoBehaviour
    {
        bool toOpen = false;


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
            BaloonControlPanel.OnStarted += Close;
            BaloonControlPanel.OnStopped += Open;
            BasePlatform.OnLanding += HandleOnLanding;

        }

        private void OnDisable()
        {
            BaloonControlPanel.OnStarted -= Close;
            BaloonControlPanel.OnStopped -= Open;
            BasePlatform.OnLanding -= HandleOnLanding;
        }

        private void HandleOnLanding(BasePlatform platform)
        {
            if (toOpen)
            {
                toOpen = false;
                transform.DOKill();
                transform.DOLocalRotate(Vector3.forward * 160f, .5f).SetEase(Ease.InOutSine);
            }
        }

        void Close()
        {
            transform.DOKill();
            transform.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.InOutSine);
        }

        void Open()
        {
            //if (BoilerController.Instance.GasLeft == 0) return; // No need to open since you die if you remain with no gas

            if (BasePlatform.CurrentPlatform)
            {
                transform.DOKill();
                transform.DOLocalRotate(Vector3.forward * 160f, .5f).SetEase(Ease.InOutSine);
            }
            else // This was the old code to manage door opening if you remain with no gas (so engine stops) while landing
            {
                toOpen = true;
            }
            
     
        }
    }
}