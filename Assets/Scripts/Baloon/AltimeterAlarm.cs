using Baloon;
using System;
using System.Globalization;
using UnityEngine;

public class AltimeterAlarm : MonoBehaviour
{
    [SerializeField]
    AudioSource redAudioSource, yellowAudioSource;

    bool started = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (!started)
        {
            //StopAlarms();
            return;
        }

        

        var currentAltitude = BaloonController.Instance.Altitude;
        var minAltitude = AltitudeManager.Instance.MinAltitude;
        var maxAltitude = AltitudeManager.Instance.MaxAltitude;

        AltitudeRange currentRange = AltitudeManager.Instance.GetCurrentRange();

        switch (currentRange)
        {
            case AltitudeRange.Red:
                PlayRedAlarm();
                break;
            case AltitudeRange.Yellow:
                StopAlarms();
                //PlayYellowAlarm();
                break;
            case AltitudeRange.Green:
                StopAlarms();
                break;
        }
    }

    private void OnEnable()
    {
        //BaloonControlPanel.OnStarted += HandleOnBaloonStarted;
        BaloonControlPanel.OnStopped += HandleOnBaloonStopped;
        BasePlatform.OnLanding += HandleOnLanding;
        BasePlatform.OnTakeOff += HandleOnTakeOff;
    }

    private void OnDisable()
    {
        //BaloonControlPanel.OnStarted -= HandleOnBaloonStarted;
        BaloonControlPanel.OnStopped -= HandleOnBaloonStopped;
        BasePlatform.OnLanding -= HandleOnLanding;
        BasePlatform.OnTakeOff -= HandleOnTakeOff;
    }

    private void HandleOnLanding(BasePlatform platform)
    {
        HandleOnBaloonStopped();
    }

    private void HandleOnTakeOff(BasePlatform platform)
    {
        HandleOnBaloonStarted();
    }

    private void HandleOnBaloonStarted()
    {
        started = true;
    }

    private void HandleOnBaloonStopped()
    {
        started = false;
        StopAlarms();
    }

    void PlayYellowAlarm()
    {
        if(redAudioSource.isPlaying) redAudioSource.Stop();
        if (!yellowAudioSource.isPlaying) yellowAudioSource.Play();
    }

    void PlayRedAlarm()
    {
        //if (yellowAudioSource.isPlaying) yellowAudioSource.Stop();
        if (!redAudioSource.isPlaying) redAudioSource.Play();
    }

    void StopAlarms()
    {
        //if (yellowAudioSource.isPlaying) yellowAudioSource.Stop();
        Debug.Log("TEST - Stoppinga alarm");
        if (redAudioSource.isPlaying) redAudioSource.Stop();
    }



}
