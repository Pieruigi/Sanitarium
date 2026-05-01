using Baloon;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Baloon
{

    public class HoldLever : MonoBehaviour
    {
        public UnityAction OnPushed;
        public UnityAction OnReleased;

        [SerializeField]
        Interactor interactor;

        [SerializeField]
        float angle = 90;

        [SerializeField]
        bool locked = false;
        public bool Locked
        {
            get { return locked; }
            set { locked = value; }
        }


        float defaultAngle;

        

        protected virtual void Awake()
        {
            defaultAngle = transform.localEulerAngles.z;
            
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

        protected virtual void Release(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            // Compute target angle
            var target = transform.localEulerAngles;
            target.z = defaultAngle;

            transform.DOKill();
            transform.DOLocalRotate(target, .1f).SetEase(Ease.OutBack);//.OnComplete(() => { OnReleased(); });

            if (locked) return;

            OnReleased?.Invoke();
        }

        protected virtual void Push(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            // Compute target angle
            var target = transform.localEulerAngles;
            target.z = angle;

            transform.DOKill();
            transform.DOLocalRotate(target, .1f).SetEase(Ease.OutBack);//.OnComplete(() => { OnPushed(); });


            if (locked) return;
            OnPushed?.Invoke();
        }

    }
}