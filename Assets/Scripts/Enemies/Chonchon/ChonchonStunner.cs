using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;

namespace Baloon
{
    public class ChonchonStunner : MonoBehaviour
    {
        [SerializeField]
        HoldButton button;

        //[SerializeField]
        //Material material;

        MaterialPropertyBlock mpb;
        Renderer _renderer;
        string baseColorPropName = "_BaseColor";

        Vector4 defaultColor;

        Sequence tween;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            mpb = new MaterialPropertyBlock();
            mpb.SetColor(baseColorPropName, new Vector4(.6f, .6f, .6f, 1));
            //_renderer.GetPropertyBlock(mpb, 0);
            _renderer.SetPropertyBlock(mpb, 0);
            defaultColor = mpb.GetColor(baseColorPropName);
        }

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
            button.OnPushed += HandleOnPushed;
            ChonchonController.OnSpawned += HandleOnSpawned;
            ChonchonController.OnStunned += HandleOnStunned;
        }

        private void OnDisable()
        {
            button.OnPushed -= HandleOnPushed;
            ChonchonController.OnSpawned -= HandleOnSpawned;
            ChonchonController.OnStunned -= HandleOnStunned;
        }

        private void HandleOnSpawned(ChonchonController chonchon)
        {
            mpb.SetColor(baseColorPropName, defaultColor * 2f);
            _renderer.SetPropertyBlock(mpb, 0);

            HandleOnStunned(false);
        }

        private void HandleOnStunned(bool stunned)
        {
            if (tween != null) tween.Kill();

            if (!stunned)
            {
                
                tween = DOTween.Sequence();

                Vector4 startColor = defaultColor;

                tween.Append(DOTween.To(() => startColor, x => startColor = x, defaultColor * 2, .25f));
                //tween.AppendCallback(() => { GetComponent<AudioSource>().Play(); });
                tween.AppendInterval(.5f);
                tween.Append(DOTween.To(() => startColor, x => startColor = x, defaultColor, .25f));
                tween.SetLoops(-1, LoopType.Restart);
                tween.OnUpdate(() =>
                {

                    mpb.SetColor(baseColorPropName, startColor);
                    _renderer.SetPropertyBlock(mpb, 0);
                    //GetComponent<Renderer>().SetPropertyBlock(lightController.MaterialPropertyBlock);
                }
                );
                tween.OnKill(() =>
                {
                    mpb.SetColor(baseColorPropName, defaultColor);
                    _renderer.SetPropertyBlock(mpb, 0);
                });
                //tween.OnComplete(() => { Debug.Log("TEST - On Complete..."); });
                tween.Play();
            }

        }

        private void HandleOnPushed()
        {
            // Get chonchon
            ChonchonController chonchon = FindFirstObjectByType<ChonchonController>();
            chonchon.SetStunnedState();
        }
    }
}