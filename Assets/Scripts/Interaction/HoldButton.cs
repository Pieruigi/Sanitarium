using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Baloon
{
    public class HoldButton : MonoBehaviour
    {
        public UnityAction OnPushed;
        public UnityAction OnReleased;
        
        [SerializeField]
        Interactor interactor;

        [SerializeField]
        float yOffset = .1f;

        [SerializeField]
        bool locked = false;
        public bool Locked
        {
            get { return locked; }
            set { locked = value; }
        }


        float defaultY;
        float pushY;


        
        protected virtual void Awake()
        {
            defaultY = transform.localPosition.y;
            pushY = defaultY - yOffset;
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

            

            transform.DOKill();
            transform.DOLocalMoveY(defaultY, .1f).SetEase(Ease.OutBack);//.OnComplete(() => { OnReleased(); });

            if (locked) return;

            OnReleased?.Invoke();
        }

        protected virtual void Push(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            transform.DOKill();

            transform.DOLocalMoveY(pushY, .1f).SetEase(Ease.OutBack);//.OnComplete(() => { OnPushed(); });

            if (locked) return;
            OnPushed?.Invoke();
        }
    }
}