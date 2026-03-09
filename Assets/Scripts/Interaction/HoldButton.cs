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
        float yOffset = .05f;

        //[SerializeField]
        //bool callDelegateBeforeTween = false;

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

            //if (callDelegateBeforeTween)
            OnReleased?.Invoke();

            transform.DOKill();
            transform.DOLocalMoveY(defaultY, .1f).SetEase(Ease.OutBack);//.OnComplete(() => { if(!callDelegateBeforeTween) OnReleased?.Invoke(); });
        }

        protected virtual void Push(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            //if (callDelegateBeforeTween) 
            OnPushed?.Invoke();

            transform.DOKill();

            transform.DOLocalMoveY(pushY, .1f).SetEase(Ease.OutBack);//.OnComplete(() => { if(!callDelegateBeforeTween) OnPushed?.Invoke(); });
        }
    }
}