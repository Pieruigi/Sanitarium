using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.VFX;

namespace Baloon
{
    public class BaloonShaker : Singleton<BaloonShaker>
    {
        Sequence shakeSequence;

        bool warningShake = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //    ShakeLight();
                

            //if (Input.GetKeyDown(KeyCode.C))
            //    StopWarningShake();
#endif
        }

        void ResetAngles()
        {
            var r = transform.localEulerAngles;
            r.x = r.z = 0f;
            //transform.localEulerAngles = r;
            transform.DOLocalRotate(r, 1f).SetEase(Ease.InOutSine);
        }

        //public void VerticalWindShake(float duration)
        //{
        //    var balloon = transform;
        //    var angle = Random.Range(10f, 22f);
        //    var angleX = Random.Range(.5f, 1f);
        //    var angleZ = Random.Range(.5f, 1f);

        //    if (shakeSequence != null) shakeSequence.Kill();

        //    shakeSequence = DOTween.Sequence();

        //    shakeSequence.Append(balloon.DOLocalRotate(new Vector3(0, angle, 0), duration)
        //        .SetEase(Ease.InOutSine));

        //    shakeSequence.Join(balloon.DOLocalRotate(new Vector3(angleX, transform.localEulerAngles.y, angleZ), duration / 2f)
        //      .SetEase(Ease.InOutSine)
        //      .SetLoops(2, LoopType.Yoyo));


        //    shakeSequence.OnComplete(() =>
        //    {
        //        ResetAngles();

        //    })
        //     .OnKill(() =>
        //     {
        //         ResetAngles();
        //     });
        //}

        public void ShakeLight()
        {
            
            var balloon = transform;
            var angle = Random.Range(20f, 45f) * (Random.Range(0,2) == 0 ? 1 : -1);
            var angleX = Random.Range(1f, 2f) * (Random.Range(0, 2) == 0 ? 1 : -1);
            var angleZ = Random.Range(1f, 2f) * (Random.Range(0, 2) == 0 ? 1 : -1);
            var duration = Random.Range(3.2f, 4f);

            float startY = transform.localEulerAngles.y;

            if (shakeSequence != null) shakeSequence.Kill();

            shakeSequence = DOTween.Sequence();

            shakeSequence.Append(balloon.DOBlendableLocalRotateBy(new Vector3(0, angle, 0), duration)
                .SetEase(Ease.InOutSine));

            
            shakeSequence.Join(balloon.DOBlendableLocalRotateBy(new Vector3(angleX, 0f, angleZ), duration / 2f)
              .SetEase(Ease.InOutSine)
              .SetLoops(2, LoopType.Yoyo)
              );

            
            shakeSequence.OnComplete(() =>
              {
                  ResetAngles();

              })
              .OnKill(() =>
              {
                  ResetAngles();
              });
            
        }

        public void ShakeHeavy()
        {
            var balloon = transform;
            var angleY = Random.Range(20f, 45f) * (Random.Range(0, 2) == 0 ? 1 : -1);
            var angleX = Random.Range(3f, 6f) * (Random.Range(0, 2) == 0 ? 1 : -1);
            var angleZ = Random.Range(3f, 6f) * (Random.Range(0, 2) == 0 ? 1 : -1);
            var duration = Random.Range(3.2f, 4f);

            if (shakeSequence != null) shakeSequence.Kill();

            shakeSequence = DOTween.Sequence();

           
            shakeSequence.Append(balloon.DOBlendableLocalRotateBy(new Vector3(0, angleY, 0), duration)
               .SetEase(Ease.InOutSine));


            shakeSequence.Join(balloon.DOBlendableLocalRotateBy(new Vector3(angleX, 0f, angleZ), duration / 2f)
              .SetEase(Ease.InOutSine)
              .SetLoops(2, LoopType.Yoyo)
              
              );


            shakeSequence.OnComplete(() =>
                {
                    ResetAngles();

                })
                .OnKill(() =>
                {
                    ResetAngles();
                });

          
        }

        public void ShakeHeavyForWindGust(float duration)
        {
            var balloon = transform;
            var angleY = Random.Range(30f, 55f) * (Random.Range(0, 2) == 0 ? 1 : -1);
            var angleX = Random.Range(5f, 8f) * (Random.Range(0, 2) == 0 ? 1 : -1);
            var angleZ = Random.Range(5f, 8f) * (Random.Range(0, 2) == 0 ? 1 : -1);

            if (shakeSequence != null) shakeSequence.Kill();

            shakeSequence = DOTween.Sequence();

          
            shakeSequence.Append(balloon.DOBlendableLocalRotateBy(new Vector3(0, angleY, 0), duration)
              .SetEase(Ease.InOutSine));


            shakeSequence.Join(balloon.DOBlendableLocalRotateBy(new Vector3(angleX, 0f, angleZ), duration / 2f)
              .SetEase(Ease.InOutSine)
              .SetLoops(2, LoopType.Yoyo)
              );


            shakeSequence.OnComplete(() =>
                {
                    ResetAngles();

                })
                .OnKill(() =>
                {
                    ResetAngles();
                });

           
        }

        public void StartWarningShake(float duration)
        {
            if (shakeSequence != null) shakeSequence.Kill();

            shakeSequence = DOTween.Sequence();

            shakeSequence.Append(transform.DOShakeRotation(duration, 2.5f, 20, fadeOut: false));

            
           
            shakeSequence
                .OnComplete(() => { warningShake = false; /*ResetAngles();*/ })
                .OnKill(() => { warningShake = false; /*ResetAngles();*/ });
        }

        public void StopWarningShake()
        {
            if (!warningShake) return;

            warningShake = false;

            if (shakeSequence != null) shakeSequence.Kill();

            shakeSequence = DOTween.Sequence();

            var target = transform.eulerAngles;
            target.x = target.z = 0f;

            shakeSequence.Append(transform.DORotate(target, .2f));

            shakeSequence
                .OnComplete(() => { ResetAngles(); })
                .OnKill(() => { ResetAngles(); });
        }

    }
}