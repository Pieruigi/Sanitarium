
using Baloon.UI;
using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using TMM;
using UnityEngine;

namespace Baloon
{

    public class BaloonLauncherPanel : MonoBehaviour
    {
        [SerializeField]
        ActivationTrigger activator;

        [SerializeField]
        Transform root;

        [SerializeField]
        Transform pivot;

        [SerializeField]
        HoldButton switchButton;

        [SerializeField]
        HoldButton launchButton;

        [SerializeField]
        GameObject miniBalloon;

        [SerializeField]
        AudioSource buttonAudioSource;

#if DEMO
        [SerializeField]
        bool demo_disabled = false;

#endif

        bool activated = false;
        public bool Activated => activated;

        float rootSpeed = 50f;

        GameObject player;

        float yRootDefault;

        bool inside = false;

        
        float yPivotDefault = 0;

        BaloonController baloon;

        Vector3 rootPositionDefault = Vector3.zero;

        BaloonLauncher baloonLauncher;

        bool unavailable = false;

        [SerializeField]
        int panelIndex = -1;

        private void Awake()
        {
            baloonLauncher = GetComponentInParent<BaloonLauncher>();

            if (!GetComponentInParent<BaloonLauncher>().IsPathAvailable(panelIndex))
            {
                gameObject.SetActive(false);
                return;
            }

            yRootDefault = root.position.y;
            yPivotDefault = pivot.localPosition.y;
            rootPositionDefault = root.position;

            miniBalloon.SetActive(false);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            baloon = FindFirstObjectByType<BaloonController>();

            

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if(unavailable) return;

            if (BaloonPathManager.Instance.CurrentPath != null) return;

            int action = 0; // 0:nothing; 1:activate; -1:deactivate
            var range = AltitudeManager.Instance.GetCurrentRange();

            //if (inside && range == AltitudeRange.Green && !activated)
            //    action = 1;
            //else if (inside && range != AltitudeRange.Green && activated)
            //    action = -1;

            if (inside && range != AltitudeRange.Red && !activated)
                action = 1;
            else if (inside && range == AltitudeRange.Red && activated)
                action = -1;

            if (action == 0 && !activated) return;


            if (action > 0)
            {
                activated = true;

                //currentOffset = player.transform.position.y - yRootDefault;

                root.DOKill();

                // Move pivot
                pivot.DOKill();
                pivot.DOLocalMoveY(1.5f, 1f).SetEase(Ease.OutSine);

                // Show balloon 
                miniBalloon.SetActive(true);
            }
            else if (action < 0)
            {
                activated = false;
                root.DOKill();

                // Reset pivot
                pivot.DOKill();
                pivot.DOLocalMoveY(yPivotDefault, 1f).SetEase(Ease.InSine).OnComplete(() => 
                { 
                    root.DOMove(rootPositionDefault, .5f);
                    
                    miniBalloon.SetActive(false); 
                });

            }

            if (activated)
            {
                var rootPos = root.position;
                var target = baloon.transform.position;// - currentOffset;

                rootPos = Vector3.Lerp(rootPos, target, rootSpeed * Time.deltaTime);

                root.position = rootPos;
            }





        }

        private void OnEnable()
        {
            activator.OnEnter += HandleOnEnter;
            activator.OnExit += HandleOnExit;
            switchButton.OnPushed += HandleOnSwitchPushed;
            launchButton.OnPushed += HandleOnLaunchPushed;
            BaloonPathManager.OnPathSet += HandleOnPathSet;
        }

        private void OnDisable()
        {
            activator.OnEnter -= HandleOnEnter;
            activator.OnExit -= HandleOnExit;
            switchButton.OnPushed -= HandleOnSwitchPushed;
            launchButton.OnPushed -= HandleOnLaunchPushed;
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
        }

        private void HandleOnPathSet()
        {
            activated = false;
            unavailable = true;

            // Clear old tween if any
            root.DOKill();
            pivot.DOKill();

            // Deactivate panel
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(.5f);
            seq.Append(pivot.DOLocalMoveY(yPivotDefault, 1f).SetEase(Ease.InSine));
            seq.Append(root.DOMove(rootPositionDefault, .5f));
            seq.AppendCallback(() => { miniBalloon.SetActive(false); });
            seq.AppendInterval(10);
            seq.AppendCallback(() => { unavailable = false; });

        }

        private void HandleOnLaunchPushed()
        {
#if DEMO
            if(demo_disabled)
            {
                // Call UI
                LauncherDemoUI.Instance.ShowMessage();
                return;
            }

            
#endif

            StartCoroutine(DoLauch());

            //baloonLauncher.SetPathFromCurrentDirection();

            IEnumerator DoLauch()
            {
                //HandleOnPathSet();

                // Play audio
                buttonAudioSource.Play();

                baloonLauncher.SwitchDirection(panelIndex);

                yield return new WaitForSeconds(.25f);

                baloonLauncher.SetPathFromCurrentDirection();
            }
        }

        private void HandleOnSwitchPushed()
        {
            baloonLauncher.SwitchDirection();
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