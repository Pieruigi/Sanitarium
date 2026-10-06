using UnityEngine;
using DG.Tweening;

namespace Baloon
{


    public class SwingingLamp : MonoBehaviour
    {
        
        [SerializeField] private float swingAngle = 12f;      
        [SerializeField] private float duration = 3.5f;       
        [SerializeField] private Vector3 swingAxis = Vector3.right; 

        private void Start()
        {
            StartSwinging();
        }

        private void StartSwinging()
        {
            
            transform.localRotation = Quaternion.Euler(swingAxis * -swingAngle);

            
            transform.DOLocalRotate(swingAxis * swingAngle, duration)
                .SetEase(Ease.InOutSine)       
                .SetLoops(-1, LoopType.Yoyo);  
        }

        private void OnDestroy()
        {
            
            transform.DOKill();
        }
    }
}