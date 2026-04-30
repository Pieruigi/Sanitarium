using Baloon;
using System;
using UnityEngine;

namespace Baloon
{
    public class BoilerFire : MonoBehaviour
    {
        [SerializeField]
        HoldSlider throttle;

        [SerializeField]
        ParticleSystem fireParticle;

        [SerializeField]
        Material springMaterial;

        float offIntensity = 0f;

        float onMinIntensity = 3;
        float onMaxIntensity = 8;
        bool isOn = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Debug.Log("TEST - V4:" + springMaterial.GetVector("_BaseColor"));
            springMaterial.SetVector("_BaseColor", new Vector4(1f, 1f, 1f, 1));
        }

        //// Update is called once per frame
        //void Update()
        //{

        //}

        private void OnEnable()
        {
            BaloonControlPanel.OnStarted += HandleOnStarted;
            BaloonControlPanel.OnStopped += HandleOnStopped;
            throttle.OnValueChanged += HandleOnThrottle;
        }

        private void OnDisable()
        {
            BaloonControlPanel.OnStarted -= HandleOnStarted;
            BaloonControlPanel.OnStopped -= HandleOnStopped;
            throttle.OnValueChanged -= HandleOnThrottle;
        }

        private void HandleOnThrottle(float value)
        {
            if (!isOn) return;
            var intensity = Mathf.Lerp(onMinIntensity, onMaxIntensity, value);
            springMaterial.SetVector("_BaseColor", Color.white * intensity);
        }

        private void HandleOnStarted()
        {
            isOn = true;
            fireParticle.Play();
            springMaterial.SetVector("_BaseColor", Color.white * onMinIntensity);
        }

        private void HandleOnStopped()
        {
            isOn = false;
            fireParticle.Stop();
            springMaterial.SetVector("_BaseColor", new Vector4(1,1,1,1));
        }
    }
}