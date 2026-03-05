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
            transform.DOLocalMoveY(defaultY, .1f).SetEase(Ease.OutBack).OnComplete(() => { OnReleased(); });
        }

        protected virtual void Push(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            transform.DOKill();

            transform.DOLocalMoveY(pushY, .1f).SetEase(Ease.OutBack).OnComplete(() => { OnPushed(); });
        }
    }
}