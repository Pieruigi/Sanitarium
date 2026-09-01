using DG.Tweening;
using UnityEngine;

namespace Baloon
{

    public class PathTile : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //float min = .5f;
            //float max = 1f;
            //transform.DOShakeRotation(1f, Random.Range(min, max), 4, 0f, false)
            //.SetLoops(-1, LoopType.Yoyo)
            //.SetEase(Ease.InOutSine);
            Shake();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void Shake()
        {
            float min = .5f;
            float max = 1f;
            transform.DOShakeRotation(1f, Random.Range(min, max), 4, 0f, false)
            .OnComplete(()=>Shake())
            .SetEase(Ease.InOutSine);
        }
    }
}