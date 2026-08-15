using DG.Tweening;
using SNT;
using System;
using System.ComponentModel;
using TMM;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

namespace Baloon
{
    public class BaloonLauncherCircle : MonoBehaviour
    {
        [SerializeField]
        ActivationTrigger activator;

        [SerializeField]
        GameObject balloonGroup;

        BaloonLauncher launcher;

        bool activated = false;

        bool unavailable = false;

        bool inside = false;

        GameObject balloon;


        Vector3 rootPositionDefault;

        float rootSpeed = 50f;

        private void Awake()
        {
            launcher = GetComponentInParent<BaloonLauncher>();
            balloonGroup.SetActive(false);
            rootPositionDefault = transform.position;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            balloon = GameObject.FindGameObjectWithTag("Baloon");
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            activator.OnEnter += HandleOnEnter;
            activator.OnExit += HandleOnExit;
            //switchButton.OnPushed += HandleOnSwitchPushed;
            //launchButton.OnPushed += HandleOnLaunchPushed;
            BaloonPathManager.OnPathSet += HandleOnPathSet;
        }

        private void OnDisable()
        {
            activator.OnEnter -= HandleOnEnter;
            activator.OnExit -= HandleOnExit;
            //switchButton.OnPushed -= HandleOnSwitchPushed;
            //launchButton.OnPushed -= HandleOnLaunchPushed;
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
        }

        private void HandleOnPathSet()
        {
            activated = false;
            unavailable = true;

            transform.DOKill();

            transform.DOMove(rootPositionDefault, 1f).OnComplete(() =>
            {
                balloonGroup.SetActive(false);
                unavailable = false;
            });
        }

        private void LateUpdate()
        {
            if (unavailable) return;

            if (launcher.IsDisabled) return;

            if (BaloonPathManager.Instance.CurrentPath != null) return;

            int action = 0; // 0:nothing; 1:activate; -1:deactivate
            var range = AltitudeManager.Instance.GetCurrentRange();

            if (inside && range != AltitudeRange.Red && !activated)
                action = 1;
            else if (inside && range == AltitudeRange.Red && activated)
                action = -1;

            if (action == 0 && !activated) return;

            if (action > 0)
            {
                activated = true;

                //currentOffset = player.transform.position.y - yRootDefault;

                transform.DOKill();

                // Move pivot
                //pivot.DOKill();
                //pivot.DOLocalMoveY(1.5f, 1f).SetEase(Ease.OutSine);

                // Show balloon 
                //miniBalloon.SetActive(true);
                balloonGroup.SetActive(true);
            }
            else if (action < 0)
            {
                activated = false;
                transform.DOKill();

                // Reset pivot
                //pivot.DOKill();
                transform.DOLocalMoveY(rootPositionDefault.y, 1f).SetEase(Ease.InSine).OnComplete(() =>
                {
                    //root.DOMove(rootPositionDefault, .5f);

                    //miniBalloon.SetActive(false);
                    balloonGroup.SetActive(false);
                });

            }

            if (activated)
            {
                var rootPos = transform.position;
                var target = balloon.transform.position;// - currentOffset;

                rootPos = Vector3.Lerp(rootPos, target, rootSpeed * Time.deltaTime);

                transform.position = rootPos;
            }

        }



        private void HandleOnEnter(Collider other)
        {
            inside = true;
        }

        private void HandleOnExit(Collider other)
        {
            inside = false;
        }
    }
}