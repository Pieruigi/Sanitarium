using DG.Tweening;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] Light _light;

    [SerializeField] int materialIndex;

    [SerializeField] Renderer _renderer;

    [SerializeField] Color onBaseColor;
    [SerializeField] Color offBaseColor;

    [SerializeField] float onColorIntensity;
    [SerializeField] float offColorIntensity;

    [SerializeField] bool isOn = false;
    public bool IsOn => isOn;

    string baseColorPropName = "_BaseColor";

    //float onOffDuration = 1f;

    MaterialPropertyBlock mpb;

    Sequence onOffSequence;

    private void Awake()
    {
        if (!_renderer) _renderer = GetComponent<Renderer>();
        if (!_light) _light = GetComponent<Light>();

       

        if (_renderer)
        {
            mpb = new MaterialPropertyBlock();
            _renderer.SetPropertyBlock(mpb);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_renderer)
        {
            mpb.SetColor(baseColorPropName, isOn ? (Vector4) onBaseColor * onColorIntensity : (Vector4) offBaseColor * offColorIntensity);
            _renderer.SetPropertyBlock(mpb, materialIndex);
        }

    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.X))
        {
            
            SetOn(IsOn ? false : true);

        }
#endif

    }



    public void SetOn(bool value)
    {
        if(isOn == value) return;
        isOn = value;

        if (onOffSequence!=null) onOffSequence.Kill();

        if (isOn) // Light on
        {
            Color baseColor = Color.black;
            float colorIntensity = 0;

            onOffSequence = DOTween.Sequence();

            // First flash
            if (_renderer)
            {
                // First flash
                onOffSequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.8f/1f), .05f));
                onOffSequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.8f / 1f), .05f));
            }


            // Off
            if (_renderer)
            {
                onOffSequence.Append(DOTween.To(() => baseColor, x => baseColor = x, offBaseColor, .1f));
                onOffSequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, offColorIntensity, .1f));
            }

            // Second flash
            if (_renderer)
            {
                onOffSequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.5f / 1f), .05f));
                onOffSequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.5f / 1f), .05f));
                onOffSequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.1f / 1f), .05f));
                onOffSequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.1f / 1f), .05f));
            }

            // Light on
            if (_renderer)
            {
                onOffSequence.Append(DOTween.To(() => baseColor, x => baseColor = x, onBaseColor, .2f));
                onOffSequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, onColorIntensity, .2f));
            }
                

            onOffSequence.OnUpdate(() => 
            {
                if (_renderer)
                {
                    mpb.SetColor(baseColorPropName, (Vector4) baseColor * colorIntensity);
                    _renderer.SetPropertyBlock(mpb, materialIndex);
                }
            });
            onOffSequence.Play();
        }
        else // Light off
        {
            // Local variables to hold the intermediate values during the tween
            Color baseColor = onBaseColor;
            float colorIntensity = onColorIntensity;

            onOffSequence = DOTween.Sequence();

            // 1. Initial "Burndown" - A quick surge followed by a drop
            if (_renderer)
            {
                // Brief overload spike (120% of on-state)
                onOffSequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.LerpUnclamped(offBaseColor, onBaseColor, 1.4f), .04f));
                onOffSequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.LerpUnclamped(offColorIntensity, onColorIntensity, 1.4f), .04f));
            }

            // 2. Heavy Glitch - Sudden drop to near darkness
            if (_renderer)
            {
                // Dropping to 10% of the on-state
                onOffSequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.1f), .06f));
                onOffSequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.1f), .06f));
            }

            // 3. Final Struggle - One last weak flicker
            if (_renderer)
            {
                // Short bounce back to 40%
                onOffSequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.4f), .03f));
                onOffSequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.4f), .03f));
            }

            // 4. Total Shutdown - Fading out to the offBase values
            if (_renderer)
            {
                // Smooth transition to the final "OFF" state
                onOffSequence.Append(DOTween.To(() => baseColor, x => baseColor = x, offBaseColor, .4f).SetEase(Ease.InQuad));
                onOffSequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, offColorIntensity, .4f).SetEase(Ease.InQuad));
            }

            // Update the MaterialPropertyBlock during every frame of the sequence
            onOffSequence.OnUpdate(() =>
            {
                if (_renderer)
                {
                    mpb.SetColor(baseColorPropName, (Vector4)baseColor * colorIntensity);
                    _renderer.SetPropertyBlock(mpb, materialIndex);
                }
            });

            onOffSequence.Play();
        }
    }
}
