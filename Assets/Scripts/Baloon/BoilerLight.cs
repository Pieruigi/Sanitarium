using Baloon;
using System;
using UnityEngine;

public class BoilerLight : MonoBehaviour
{
    [SerializeField]
    LightFlicker flicker;

    [SerializeField]
    LightController lightController;

    [SerializeField]
    float idleMinIntensity, idleMaxIntensity, idleSpeed;

    [SerializeField]
    float fullMinIntensity, fullMaxIntensity, fullSpeed;

    [SerializeField]
    HoldSlider throttle;

    bool running = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LightOff();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    private void HandleOnStarted()
    {
        running = true;
        LightIdle();
    }

    private void HandleOnStopped()
    {
        running = false;
        LightOff();
    }

    private void HandleOnThrottle(float value)
    {
        if (!running) return;

        float currentMin = Mathf.Lerp(idleMinIntensity, fullMinIntensity, value);
        float currentMax = Mathf.Lerp(idleMaxIntensity, fullMaxIntensity, value);
        float currentSpeed = Mathf.Lerp(idleSpeed, fullSpeed, value);

        flicker.minIntensity = currentMin;
        flicker.maxIntensity = currentMax;
        flicker.speed = currentSpeed;
    }

    void LightOff()
    {
        flicker.StopFlickering();

        lightController.SetOn(false);
    }

    void LightIdle()
    {
        lightController.SetOn(true);


        flicker.minIntensity = idleMinIntensity;
        flicker.maxIntensity = idleMaxIntensity;
        flicker.speed = idleSpeed;

        flicker.StartFlickering();
    }


}
