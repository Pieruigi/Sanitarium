using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Baloon
{
    public class Flashlight : Singleton<Flashlight>
    {
        [SerializeField]
        Transform root;

        [SerializeField]
        InputActionReference inputAction;

        [SerializeField]
        bool isOn = false;

        
        [SerializeField]
        Vector3 onPosition;
        [SerializeField]
        Vector3 onEulers;

        [SerializeField]
        Vector3 offPosition;
        [SerializeField]
        Vector3 offEulers;

        
        Sequence tween;

        bool lastPressed = false;


        protected override void Awake()
        {
            base.Awake();

            
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (isOn)
            {
                root.localPosition = onPosition;
                root.localEulerAngles = onEulers;
                root.gameObject.SetActive(true);

            }
            else
            {
                root.localPosition = offPosition;
                root.localEulerAngles = offEulers;
                root.gameObject.SetActive(false);
            }

        }

        // Update is called once per frame
        void Update()
        {
            
            if (inputAction.action.IsPressed() && !lastPressed)
            {
                if (isOn)
                {
                    isOn = false;

                    if (tween != null) tween.Kill();
                    tween = DOTween.Sequence();
                    tween.Append(root.DOLocalMove(offPosition, .25f));
                    tween.Join(root.DOLocalRotate(offEulers, .25f));
                    tween.OnComplete(() => { root.gameObject.SetActive(false); });
                }
                else
                {
                    isOn = true;
                    root.gameObject.SetActive(true);
                    if (tween != null) tween.Kill();
                    tween = DOTween.Sequence();
                    tween.Append(root.DOLocalMove(onPosition, .25f));
                    tween.Join(root.DOLocalRotate(onEulers, .25f));
                    tween.Join(root.DOLocalRotate(onEulers, .25f));
                }
            }

            lastPressed = inputAction.action.IsPressed();
        }
    }
}