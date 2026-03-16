using Cinemachine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Baloon
{
    public class FOVController : Singleton<FOVController>
    {
        float fovDefault;

        CinemachineVirtualCamera virtualCamera;

        float duration = .5f;

        Tween fovTween;

        protected override void Awake()
        {
            base.Awake();

            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            fovDefault = virtualCamera.m_Lens.FieldOfView;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        public void SetFOV(float fov)
        {
            if(fovTween != null) fovTween.Kill();

            fovTween = DOTween.To(() => virtualCamera.m_Lens.FieldOfView,
                   x => virtualCamera.m_Lens.FieldOfView = x,
                   fov,
                   duration)
               .SetEase(Ease.OutSine);
        }

        public void ResetFOV()
        {
            if (fovTween != null) fovTween.Kill();

            fovTween = DOTween.To(() => virtualCamera.m_Lens.FieldOfView,
                   x => virtualCamera.m_Lens.FieldOfView = x,
                   fovDefault,
                   duration)
               .SetEase(Ease.OutSine);
        }
    }
}