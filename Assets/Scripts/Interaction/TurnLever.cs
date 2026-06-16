using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Baloon
{
    public class TurnLever : MonoBehaviour
    {
        public UnityAction OnPushed;
        public UnityAction OnReleased;

        [SerializeField]
        Interactor interactor;


        [SerializeField] 
        float angle = 360; // Stay under 360 or replace transform.DOLocalRotate() with DOTween.TO() using a currentAngle variable

        [SerializeField]
        float duration = 3f;

        [SerializeField]
        bool locked = false;
        public bool Locked
        {
            get { return locked; }
            set { locked = value; }
        }


        float defaultAngle = 0;
        float targetAngle = 0;
        float currentAngle = 0;

        Tween turnTween;

        bool completed = false;

        public float NormalizedProgress => completed ? 1f : Mathf.Abs(currentAngle - defaultAngle) / Mathf.Abs(angle);

        private void Awake()
        {
            defaultAngle = transform.localEulerAngles.y;
            targetAngle = defaultAngle + angle;
            currentAngle = defaultAngle;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }

        protected virtual void OnEnable()
        {
            Interactor.OnInteractionStarted += Push;
            Interactor.OnInteractionStopped += Release;
        }

        protected virtual void OnDisable()
        {
            Interactor.OnInteractionStarted -= Push;
            Interactor.OnInteractionStopped -= Release;
        }

        private void Push(Interactor interactor)
        {
            if (this.interactor != interactor) return;
                        
            if (locked || completed) return;

            //if (turnTween != null && turnTween.IsPlaying()) return;

            // Start tween
            var time = MathF.Abs(currentAngle - targetAngle) / Mathf.Abs(defaultAngle - targetAngle) * duration;
            Debug.Log($"TEST - Time:{time}");
            turnTween = DOTween.To(() => currentAngle, x => currentAngle = x, targetAngle, time).SetEase(Ease.InOutSine);
            turnTween.OnUpdate(() => { transform.localEulerAngles = Vector3.up * currentAngle; });
            turnTween.OnComplete(() => { transform.localEulerAngles = Vector3.up * targetAngle; completed = true; });

            OnPushed?.Invoke();
        }

        private void Release(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            if (turnTween != null) turnTween.Kill();

            OnReleased?.Invoke();
        }

        public void ForceCompleted()
        {
            completed = true;
            transform.localEulerAngles = Vector3.up * angle;
        }
    }
}