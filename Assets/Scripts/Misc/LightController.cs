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
        }


        [SerializeField] Light _light;

        [SerializeField] int materialIndex;

        [SerializeField] Renderer _renderer;

        [SerializeField] Color onBaseColor;
        [SerializeField] Color offBaseColor;

        [SerializeField] float onColorIntensity;
        [SerializeField] float offColorIntensity;

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

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.X))
            {

                SwitchData(1);
                //SetOn(IsOn ? false : true);

            }
#endif

        }

        void InitData()
        {
            onColorIntensity = data[dataIndex].colorIntensity;
            onBaseColor = data[dataIndex].baseColor;

        }

        public void SetOn(bool value)
        {
            if (isOn == value) return;
            isOn = value;

            if (sequence != null) sequence.Kill();

            if (isOn) // Light on
            {
                Color baseColor = Color.black;
                float colorIntensity = 0;

                sequence = DOTween.Sequence();

                // First flash
                if (_renderer)
                {
                    // First flash
                    sequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.8f / 1f), .05f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.8f / 1f), .05f));
                }


                // Off
                if (_renderer)
                {
                    sequence.Append(DOTween.To(() => baseColor, x => baseColor = x, offBaseColor, .1f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, offColorIntensity, .1f));
                }

                // Second flash
                if (_renderer)
                {
                    sequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.5f / 1f), .05f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.5f / 1f), .05f));
                    sequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.1f / 1f), .05f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.1f / 1f), .05f));
                }

                // Light on
                if (_renderer)
                {
                    sequence.Append(DOTween.To(() => baseColor, x => baseColor = x, onBaseColor, .2f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, onColorIntensity, .2f));
                }


                sequence.OnUpdate(() =>
                {
                    if (_renderer)
                    {
                        mpb.SetColor(baseColorPropName, (Vector4)baseColor * colorIntensity);
                        _renderer.SetPropertyBlock(mpb, materialIndex);
                    }
                });
                sequence.Play();
            }
            else // Light off
            {
                // Local variables to hold the intermediate values during the tween
                Color baseColor = onBaseColor;
                float colorIntensity = onColorIntensity;

                sequence = DOTween.Sequence();

                // 1. Initial "Burndown" - A quick surge followed by a drop
                if (_renderer)
                {
                    // Brief overload spike (120% of on-state)
                    sequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.LerpUnclamped(offBaseColor, onBaseColor, 1.4f), .04f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.LerpUnclamped(offColorIntensity, onColorIntensity, 1.4f), .04f));
                }

                // 2. Heavy Glitch - Sudden drop to near darkness
                if (_renderer)
                {
                    // Dropping to 10% of the on-state
                    sequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.1f), .06f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.1f), .06f));
                }

                // 3. Final Struggle - One last weak flicker
                if (_renderer)
                {
                    // Short bounce back to 40%
                    sequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(offBaseColor, onBaseColor, 0.4f), .03f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, Mathf.Lerp(offColorIntensity, onColorIntensity, 0.4f), .03f));
                }

                // 4. Total Shutdown - Fading out to the offBase values
                if (_renderer)
                {
                    // Smooth transition to the final "OFF" state
                    sequence.Append(DOTween.To(() => baseColor, x => baseColor = x, offBaseColor, .4f).SetEase(Ease.InQuad));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, offColorIntensity, .4f).SetEase(Ease.InQuad));
                }

                // Update the MaterialPropertyBlock during every frame of the sequence
                sequence.OnUpdate(() =>
                {
                    if (_renderer)
                    {
                        mpb.SetColor(baseColorPropName, (Vector4)baseColor * colorIntensity);
                        _renderer.SetPropertyBlock(mpb, materialIndex);
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

                sequence = DOTween.Sequence();

                if (_renderer)
                {
                    sequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.black, 0.08f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, colorIntensity * 1.6f, 0.08f));
                }

                // 2. THE FLICKER: Rapid instability during the color swap
                if (_renderer)
                {
                    // First drop: nearly dark to create a sharp contrast
                    sequence.Append(DOTween.To(() => colorIntensity, x => colorIntensity = x, onColorIntensity * 0.2f, 0.04f));

                    // Quick bounce: moving towards the target color but still unstable
                    sequence.Append(DOTween.To(() => baseColor, x => baseColor = x, Color.Lerp(baseColor, onBaseColor, 0.5f), 0.03f));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, onColorIntensity * 0.8f, 0.03f));

                    // Second drop: a smaller glitch
                    sequence.Append(DOTween.To(() => colorIntensity, x => colorIntensity = x, onColorIntensity * 0.4f, 0.03f));
                }

                // 3. SETTLING: Final transition to the new state
                if (_renderer)
                {
                    //// 2. Settle into the new color with a nice bounce (using your OutCubic)
                    sequence.Append(DOTween.To(() => baseColor, x => baseColor = x, onBaseColor, 0.25f).SetEase(Ease.OutCubic));
                    sequence.Join(DOTween.To(() => colorIntensity, x => colorIntensity = x, onColorIntensity, 0.25f));
                }


                sequence.OnUpdate(() =>
                {
                    if (_renderer)
                    {
                        mpb.SetColor(baseColorPropName, (Vector4)baseColor * colorIntensity);
                        _renderer.SetPropertyBlock(mpb, materialIndex);
                    }
                });

                sequence.Play();

            }

            
            


        }

    }
}