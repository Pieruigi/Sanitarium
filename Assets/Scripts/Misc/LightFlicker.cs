using UnityEngine;
using DG.Tweening;

public class LightFlicker : MonoBehaviour
{
    private Light _light;
    private float _baseIntensity;
    private float _baseRange;

    [Header("Settings")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;

    

    // Lower speed means more frantic/fast flickering
    public float speed = 0.1f;



    void Awake()
    {
        _light = GetComponent<Light>();
        if (_light != null)
        {
            _baseIntensity = _light.intensity;
            _baseRange = _light.range;
        }
    }

    void Start()
    {
        //if (_light != null)
        //{
        //    StartFlicker();
        //}
    }

    public void StartFlickering()
    {
        // Generate a random intensity and duration to avoid a predictable pattern
        float targetIntensity = Random.Range(minIntensity, maxIntensity);
        float randomDuration = Random.Range(speed * 0.5f, speed * 1.5f);

        // Perform the flicker using DOTween
        _light.DOIntensity(targetIntensity, randomDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(StartFlickering)
            .SetId(this); // Assign an ID to safely kill the tween later

        //// Range
        //float intensityPercentage = targetIntensity / maxIntensity;
        //float targetRange = _baseRange * intensityPercentage;
        //DOTween.To(() => _light.range, x => _light.range = x, targetRange, randomDuration)
        //.SetEase(Ease.InOutSine)
        //.SetId(this);

    }

    private void OnDisable()
    {
        // Stop the flicker when the object or script is disabled to prevent errors
        DOTween.Kill(this);
    }

    private void OnEnable()
    {
        if (_light != null)
        {
            StartFlickering();
        }
        
    }

    public void ToggleLight(bool state)
    {
        _light.enabled = state;
        if (state) StartFlickering();
        else DOTween.Kill(this);
    }

    public void StopFlickering()
    {
        DOTween.Kill(this);
    }
}