using DG.Tweening;
using StarterAssets;
using UnityEngine;

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
            //    StartWarningShake();

            //if (Input.GetKeyDown(KeyCode.C))
            //    StopWarningShake();
#endif
        }

        void ResetAngles()
        {
            var r = transform.localEulerAngles;
            r.x = r.z = 0f;
            transform.localEulerAngles = r;
        }

        public void ShakeLight()
        {
            
            var balloon = transform;
            var angle = Random.Range(20f, 45f);
            var angleX = Random.Range(1f, 2f);
            var angleZ = Random.Range(1f, 2f);
            var duration = Random.Range(3.2f, 4f);

            if (shakeSequence != null) shakeSequence.Kill();

            shakeSequence = DOTween.Sequence();

            shakeSequence.Append(balloon.DOLocalRotate(new Vector3(0, angle, 0), duration)
                .SetEase(Ease.InOutSine));

            shakeSequence.Join(balloon.DOLocalRotate(new Vector3(angleX, 0f, angleZ), duration / 2f)
              .SetEase(Ease.InOutSine)
              .SetLoops(2, LoopType.Yoyo));

            
             shakeSequence.OnComplete(() =>
              {
                  ResetAngles();

              })
              .OnKill(() =>
              {
                  ResetAngles();
              });

            //void ResetAngles()
            //{
            //    var r = balloon.localEulerAngles;
            //    r.x = r.z = 0f;
            //    balloon.localEulerAngles = r;
            //}
        }

        public void ShakeHeavy()
        {
            var balloon = transform;
            var angleY = Random.Range(20f, 45f);
            var angleX = Random.Range(3f, 6f);
            var angleZ = Random.Range(3f, 6f);
            var duration = Random.Range(3.2f, 4f);

            if (shakeSequence != null) shakeSequence.Kill();

            shakeSequence = DOTween.Sequence();

            shakeSequence.Append(balloon.DOLocalRotate(new Vector3(0f, angleY, 0f), duration));
            shakeSequence.Join(balloon.DOLocalRotate(new Vector3(angleX, 0f, angleZ), duration / 2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(2, LoopType.Yoyo));
            

            
            shakeSequence.OnComplete(() =>
                {
                    ResetAngles();

                })
                .OnKill(() =>
                {
                    ResetAngles();
                });

            //void ResetAngles()
            //{
            //    var r = balloon.localEulerAngles;
            //    r.x = r.z = 0f;
            //    balloon.localEulerAngles = r;
            //}
        }

        public void ShakeHeavyForWindGust(float duration)
        {
            var balloon = transform;
            var angleY = Random.Range(30f, 55f);
            var angleX = Random.Range(5f, 8f);
            var angleZ = Random.Range(5f, 8f);

            if (shakeSequence != null) shakeSequence.Kill();

            shakeSequence = DOTween.Sequence();

            shakeSequence.Append(balloon.DOLocalRotate(new Vector3(0f, angleY, 0f), duration));
            shakeSequence.Join(balloon.DOLocalRotate(new Vector3(angleX, 0f, angleZ), duration / 2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(2, LoopType.Yoyo));
            

            
            shakeSequence.OnComplete(() =>
                {
                    ResetAngles();

                })
                .OnKill(() =>
                {
                    ResetAngles();
                });

            //void ResetAngles()
            //{
            //    var r = balloon.localEulerAngles;
            //    r.x = r.z = 0f;
            //    balloon.localEulerAngles = r;
            //}
        }

        public void StartWarningShake()
        {
            if (shakeSequence != null) shakeSequence.Kill();

            shakeSequence = DOTween.Sequence();

            shakeSequence.Append(transform.DOShakeRotation(4f, 2.5f, 20, fadeOut: false));

            //float duration = 1f;
            //float strength = 1f;
            //int vibrato = 10;

            //for(int i=0; i<6; i++)
            //{
            //    shakeSequence.Append(transform.DOShakeRotation(duration, strength, vibrato, fadeOut: false));
            //    duration *= .8f;
            //    strength *= 1.2f;
            //    vibrato = Mathf.RoundToInt(vibrato * 1.2f);
            //}

          

           
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