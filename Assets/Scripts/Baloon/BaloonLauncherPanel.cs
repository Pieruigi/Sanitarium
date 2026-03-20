
using DG.Tweening;
using System;
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

        bool activated = false;
        public bool Activated => activated;

        float rootSpeed = 50f;

        GameObject player;

        float yRootDefault;

        bool inside = false;

        float currentOffset = 0;

        float yPivotDefault = 0;

        BaloonController baloon;

        Vector3 rootPositionDefault = Vector3.zero;

        BaloonLauncher baloonLauncher;

        private void Awake()
        {
            yRootDefault = root.position.y;
            yPivotDefault = pivot.localPosition.y;
            rootPositionDefault = root.position;
            baloonLauncher = GetComponentInParent<BaloonLauncher>();
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
            int action = 0; // 0:nothing; 1:activate; -1:deactivate
            var range = AltitudeManager.Instance.GetCurrentRange();

            if (inside && range == AltitudeRange.Green && !activated)
                action = 1;
            else if (inside && range != AltitudeRange.Green && activated)
                action = -1;

            if (action == 0 && !activated) return;


            if (action > 0)
            {
                activated = true;

                currentOffset = player.transform.position.y - yRootDefault;

                root.DOKill();

                // Move pivot
                pivot.DOKill();
                pivot.DOLocalMoveY(1.5f, 1f).SetEase(Ease.OutSine);


            }
            else if (action < 0)
            {
                activated = false;
                root.DOKill();

                // Reset pivot
                pivot.DOKill();
                pivot.DOLocalMoveY(yPivotDefault, 1f).SetEase(Ease.InSine).OnComplete(() => { root.DOMove(rootPositionDefault, .5f); });

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
            
        }

        private void OnDisable()
        {
            activator.OnEnter -= HandleOnEnter;
            activator.OnExit -= HandleOnExit;
            switchButton.OnPushed -= HandleOnSwitchPushed;
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