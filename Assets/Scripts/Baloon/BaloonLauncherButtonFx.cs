using DG.Tweening;
using UnityEngine;

namespace Baloon
{
    public class BaloonLauncherButtonFx : MonoBehaviour
    {
        bool playing = false;


        Vector4 defaultColor;

        LightController lightController;

        string baseColorPropName = "_BaseColor";

        Sequence tween;

        private void Awake()
        {
            //enabled = false;
            lightController = transform.parent.GetComponent<LightController>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Play()
        {
            if (playing) return;

            defaultColor = lightController.MaterialPropertyBlock.GetColor(baseColorPropName);

            playing = true;

            tween = DOTween.Sequence();

            Vector4 startColor = defaultColor;

            tween.Append(DOTween.To(() => startColor, x => startColor = x, defaultColor * 2, .5f));
            tween.AppendCallback(() => { GetComponent<AudioSource>().Play(); });
            tween.AppendInterval(1f);
            tween.Append(DOTween.To(() => startColor, x => startColor = x, defaultColor, .5f));
            tween.SetLoops(-1, LoopType.Restart);
            tween.OnUpdate(() => 
            {
                
                lightController.ForceColor(startColor);
                //GetComponent<Renderer>().SetPropertyBlock(lightController.MaterialPropertyBlock);
            }
            );
            tween.OnKill(() => 
            {
                lightController.ForceColor((Vector4)defaultColor);
            });
            //tween.OnComplete(() => { Debug.Log("TEST - On Complete..."); });
            tween.Play();
        }

        public void Stop()
        {
            if (!playing) return;

            playing = false;

            //lightController.MaterialPropertyBlock.SetColor(baseColorPropName, (Vector4)defaultColor);
            if(tween != null) tween.Kill();
        }
    }
}