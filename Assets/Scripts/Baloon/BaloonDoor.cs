using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Baloon
{
    public class BaloonDoor : MonoBehaviour
    {



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
            BaloonControlPanel.OnStarted += Close;
            BaloonControlPanel.OnStopped += Open;

        }

        private void OnDisable()
        {
            BaloonControlPanel.OnStarted -= Close;
            BaloonControlPanel.OnStopped -= Open;
        }

        void Close()
        {
            transform.DOKill();
            transform.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.InOutSine);
        }

        void Open()
        {
            transform.DOKill();
            transform.DOLocalRotate(Vector3.up * 160f, .5f).SetEase(Ease.InOutSine);
     
        }
    }
}