using DG.Tweening;
using System;
using UnityEngine;

namespace SNT
{
    public class TriggerButtonEffect : MonoBehaviour
    {
        [SerializeField]
        TriggerButton triggerButton;

        [SerializeField]
        Transform target;

        float defaultY;
        float pressedY = -.025f;

        Sequence tweenSeq;

        private void Awake()
        {
            if (!target) target = transform;
            if(!triggerButton) triggerButton = GetComponent<TriggerButton>();

            defaultY = target.localPosition.y;
            pressedY += defaultY;
        }

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
            TriggerButton.OnTriggered += HandleOnTriggered;
        }

        private void OnDisable()
        {
            TriggerButton.OnTriggered -= HandleOnTriggered;
        }

        private void HandleOnTriggered(TriggerButton triggerButton)
        {
            if(this.triggerButton != triggerButton) return;

            Debug.Log("TEST - Pressing button...");
            tweenSeq.Kill();
            tweenSeq = DOTween.Sequence();
            tweenSeq.Append(target.DOLocalMoveY(pressedY, .25f).SetEase(Ease.OutBack));
            tweenSeq.Append(target.DOLocalMoveY(defaultY, .25f).SetEase(Ease.InBack));
        }
    }
}