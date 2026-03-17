using DG.Tweening;
using UnityEngine;

namespace Baloon
{
    public class LightController : MonoBehaviour
    {
        [System.Serializable]
        class LightOnData
        {
            public Color baseColor;
            public float colorIntensity;
            public Color lightColor;
            public float lightIntensity; 
        }


        [SerializeField] Light _light;

        [SerializeField] int materialIndex;

        [SerializeField] Renderer _renderer;

        Color onBaseColor;
        [SerializeField] Color offBaseColor;

        float onColorIntensity;
        [SerializeField] float offColorIntensity;

        Color onLightColor;
        float onLightIntensity;
        Color offLightColor = Color.white;
        float offLightIntensity = 0;

        [SerializeField] bool isOn = false;

        [SerializeField]
        LightOnData[] data;

        [SerializeField]
        int dataIndex = 0;
        public bool IsOn => isOn;

        string baseColorPropName = "_BaseColor";

        //float onOffDuration = 1f;

        MaterialPropertyBlock mpb;

        Sequence sequence;

        private void Awake()
        {
            if (!_renderer) _renderer = GetComponent<Renderer>();
            if (!_light) _light = GetComponent<Light>();

            InitData();
            

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
                mpb.SetColor(baseColorPropName, isOn ? (Vector4)onBaseColor * onColorIntensity : (Vector4)offBaseColor * offColorIntensity);
                _renderer.SetPropertyBlock(mpb, materialIndex);
            }
            if (_light)
            {
                _light.color = isOn ? onLightColor : offLightColor;
                _light.intensity = isOn ? onLightIntensity : offLightIntensity;
            }
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //{

            //    SwitchData(1);
            //    //SetOn(IsOn ? false : true);

            //}
#endif

        }

        void InitData()
        {
            onColorIntensity = data[dataIndex].colorIntensity;
            onBaseColor = data[dataIndex].baseColor;
            onLightColor = data[dataIndex].lightColor;
            onLightIntensity = data[dataIndex].lightIntensity;
        }

        public void SetOn(bool value)
        {
            if (isOn == value) return;
            isOn = value;

            if (sequence != null) sequence.Kill();

            if (isOn) // Light on
            {
                Color baseColor = offBaseColor;
                float colorIntensity = offColorIntensity;
                Color lightColor = offLightColor;
                float lightIntensity = offLightIntensity;

                sequence = DOTween.Sequence();

                sequence.AppendInterval(0);

                // First flash
                if (_renderer)
                {
                    // First flash
                    sequence.Join(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.8f / 1f), .05f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.8f / 1f), .05f));
                }
                if (_light)
                {
                    // First flash
                    sequence.Join(DOTween.To(() => lightColor, x => lightColor = x, Color.Lerp(offLightColor, onLightColor, 0.8f / 1f), .05f));
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, Mathf.Lerp(offLightIntensity, onLightIntensity, 0.8f / 1f), .05f));
                }

                sequence.AppendInterval(0);

                // Off
                if (_renderer)
                {
                    sequence.Join(DOTween.To(() => baseColor, x => baseColor = x, offBaseColor, .1f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, offColorIntensity, .1f));
                }
                if (_light)
                {
                    sequence.Join(DOTween.To(() => lightColor, x => lightColor = x, offLightColor, .1f));
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, offLightIntensity, .1f));
                }

                sequence.AppendInterval(0);

                // Second flash
                if (_renderer)
                {
                    sequence.Join(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.5f / 1f), .05f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.5f / 1f), .05f));
                 
                }
                if (_light)
                {
                    sequence.Join(DOTween.To(() => lightColor, x => lightColor = x, Color.Lerp(offLightColor, onLightColor, 0.5f / 1f), .05f));
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, Mathf.Lerp(offLightIntensity, onLightIntensity, 0.5f / 1f), .05f));

                }
                sequence.AppendInterval(0);

                if (_renderer)
                {
                    sequence.Join(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.1f / 1f), .05f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.1f / 1f), .05f));
                }
                if (_light)
                {
                    sequence.Join(DOTween.To(() => lightColor, x => lightColor = x, Color.Lerp(offLightColor, onLightColor, 0.1f / 1f), .05f));
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, Mathf.Lerp(offLightIntensity, onLightIntensity, 0.1f / 1f), .05f));
                }

                sequence = sequence.AppendInterval(0);
                // Light on
                if (_renderer)
                {
                    sequence.Join(DOTween.To(() => baseColor, x => baseColor = x, onBaseColor, .2f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, onColorIntensity, .2f));
                }
                if (_light)
                {
                    sequence.Join(DOTween.To(() => lightColor, x => lightColor = x, onLightColor, .2f));
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, onLightIntensity, .2f));
                }


                sequence.OnUpdate(() =>
                {
                    if (_renderer)
                    {
                        mpb.SetColor(baseColorPropName, (Vector4)baseColor * colorIntensity);
                        _renderer.SetPropertyBlock(mpb, materialIndex);
                    }
                    if (_light)
                    {
                        _light.color = lightColor;
                        _light.intensity = lightIntensity;
                    }
                });
                sequence.Play();
            }
            else // Light off
            {
                // Local variables to hold the intermediate values during the tween
                Color baseColor = onBaseColor;
                float colorIntensity = onColorIntensity;
                Color lightColor = onLightColor;
                float lightIntensity = onLightIntensity;  

                sequence = DOTween.Sequence();

                sequence.AppendInterval(0);

                // 1. Initial "Burndown" - A quick surge followed by a drop
                if (_renderer)
                {
                    // Brief overload spike (120% of on-state)
                    sequence.Join(DOTween.To(() => baseColor, x => baseColor = x, Color.LerpUnclamped(offBaseColor, onBaseColor, 1.4f), .04f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.LerpUnclamped(offColorIntensity, onColorIntensity, 1.4f), .04f));
                }
                if (_light)
                {
                    // Brief overload spike (120% of on-state)
                    sequence.Join(DOTween.To(() => lightColor, x => lightColor = x, Color.LerpUnclamped(offLightColor, onLightColor, 1.4f), .04f));
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, Mathf.LerpUnclamped(offLightIntensity, onLightIntensity, 1.4f), .04f));
                }

                sequence.AppendInterval(0);

                // 2. Heavy Glitch - Sudden drop to near darkness
                if (_renderer)
                {
                    // Dropping to 10% of the on-state
                    sequence.Join(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.1f), .06f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.1f), .06f));
                }
                if (_light)
                {
                    // Dropping to 10% of the on-state
                    sequence.Join(DOTween.To(() => lightColor, x => lightColor = x, Color.Lerp(offLightColor, onLightColor, 0.1f), .06f));
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, Mathf.Lerp(offLightIntensity, onLightIntensity, 0.1f), .06f));
                }
                sequence.AppendInterval(0);

                // 3. Final Struggle - One last weak flicker
                if (_renderer)
                {
                    // Short bounce back to 40%
                    sequence.Join(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.4f), .03f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.4f), .03f));
                }
                if (_light)
                {
                    // Short bounce back to 40%
                    sequence.Join(DOTween.To(() => lightColor, x => lightColor = x, Color.Lerp(offLightColor, onLightColor, 0.4f), .03f));
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, Mathf.Lerp(offLightIntensity, onLightIntensity, 0.4f), .03f));
                }

                sequence.AppendInterval(0);
                // 4. Total Shutdown - Fading out to the offBase values
                if (_renderer)
                {
                    // Smooth transition to the final "OFF" state
                    sequence.Join(DOTween.To(() => baseColor, x => baseColor = x, offBaseColor, .4f).SetEase(Ease.InQuad));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, offColorIntensity, .4f).SetEase(Ease.InQuad));
                }
                if (_light)
                {
                    // Smooth transition to the final "OFF" state
                    sequence.Join(DOTween.To(() => lightColor, x => lightColor = x, offLightColor, .4f).SetEase(Ease.InQuad));
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, offLightIntensity, .4f).SetEase(Ease.InQuad));
                }

                // Update the MaterialPropertyBlock during every frame of the sequence
                sequence.OnUpdate(() =>
                {
                    if (_renderer)
                    {
                        mpb.SetColor(baseColorPropName, (Vector4)baseColor * colorIntensity);
                        _renderer.SetPropertyBlock(mpb, materialIndex);
                    }
                    if (_light)
                    {
                        _light.color = lightColor;
                        _light.intensity = lightIntensity;
                    }
                });

                sequence.Play();
            }
        }

        public void SwitchData(int newIndex)
        {
            if (dataIndex == newIndex) return;

            int oldIndex = dataIndex;
            dataIndex = newIndex;

            InitData();

            if (isOn)
            {
                // If is on we tween between old and new value
                if (sequence != null) sequence.Kill();

                var baseColor = data[oldIndex].baseColor;
                var colorIntensity = data[oldIndex].colorIntensity;
                var lightColor = data[oldIndex].lightColor;
                var lightIntensity = data[oldIndex].lightIntensity;

                sequence = DOTween.Sequence();

                sequence.AppendInterval(0);

                if (_renderer)
                {
                    sequence.Join(DOTween.To(() => baseColor, x => baseColor = x, Color.black, 0.08f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, colorIntensity * 1.6f, 0.08f));
                }
                if (_light)
                {
                    sequence.Join(DOTween.To(() => lightColor, x => lightColor = x, Color.black, 0.08f));
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, colorIntensity * 1.6f, 0.08f));
                }

                sequence.AppendInterval(0);

                // 2. THE FLICKER: Rapid instability during the color swap
                if (_renderer)
                {
                    // First drop: nearly dark to create a sharp contrast
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, onColorIntensity * 0.2f, 0.04f));
                }
                if (_light)
                {
                    // First drop: nearly dark to create a sharp contrast
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, onLightIntensity * 0.2f, 0.04f));
                }

                sequence.AppendInterval(0);
                if (_renderer)
                {
                    
                    // Quick bounce: moving towards the target color but still unstable
                    sequence.Join(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(baseColor, onBaseColor, 0.5f), 0.03f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, onColorIntensity * 0.8f, 0.03f));
                }
                if (_light)
                {
                    // Quick bounce: moving towards the target color but still unstable
                    sequence.Join(DOTween.To(() => lightColor, x => lightColor = x, Color.Lerp(lightColor, onLightColor, 0.5f), 0.03f));
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, onLightIntensity * 0.8f, 0.03f));
                }

                sequence.AppendInterval(0);

                if (_renderer)
                {
                    
                    // Second drop: a smaller glitch
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, onColorIntensity * 0.4f, 0.03f));
                }
                if (_light)
                {

                    // Second drop: a smaller glitch
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, onLightIntensity * 0.4f, 0.03f));
                }

                sequence.AppendInterval(0);

                // 3. SETTLING: Final transition to the new state
                if (_renderer)
                {
                    //// 2. Settle into the new color with a nice bounce (using your OutCubic)
                    sequence.Join(DOTween.To(() => baseColor, x => baseColor = x, onBaseColor, 0.25f).SetEase(Ease.OutCubic));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, onColorIntensity, 0.25f));
                }
                if (_light)
                {
                    //// 2. Settle into the new color with a nice bounce (using your OutCubic)
                    sequence.Join(DOTween.To(() => lightColor, x => lightColor = x, onLightColor, 0.25f).SetEase(Ease.OutCubic));
                    sequence.Join(DOTween.To(() => lightIntensity, x => lightIntensity = x, onLightIntensity, 0.25f));
                }


                sequence.OnUpdate(() =>
                {
                    if (_renderer)
                    {
                        mpb.SetColor(baseColorPropName, (Vector4)baseColor * colorIntensity);
                        _renderer.SetPropertyBlock(mpb, materialIndex);
                    }
                    if (_light)
                    {
                        _light.color = lightColor;
                        _light.intensity = lightIntensity;
                    }
                });

                sequence.Play();

            }

            
            


        }

    }
}