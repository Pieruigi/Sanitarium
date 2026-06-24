using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace Baloon
{
    public class SettingsManager : SingletonPersistent<SettingsManager>
    {
        public delegate void OptionsChangedDelegate();
        public static OptionsChangedDelegate OnOptionsChanged;

        public static UnityAction OnResolutionChanged;

        [SerializeField]
        AudioMixer mixer;

        public const string MouseSpeedOptionParam = "MouseSpeed";

        public const int MouseSpeedOptionMin = 1;
        public const int MouseSpeedOptionMax = 9;

        public const int MouseSpeedOptionDefault = 5;

        public const string VolumeOptionParam = "Volume";

        //public const int VolumeOptionMin = 0;
        //public const int VolumeOptionMax = 100;

        public const int VolumeOptionDefault = 80;

        public const string VerticalMouseOptionParam = "VerticalMouse";

        public const int VerticalMouseOptionDefault = 0;

        //public const string ResolutionIdOptionParam = "ResolutionId";

       

        public float MouseSpeed
        {
            get
            {
                var v = PlayerPrefs.GetInt("MouseSpeed", MouseSpeedOptionDefault);
                if (v == 5)
                {
                    return 1.0f;
                }
                else if (v > 5)
                {
                    // Scala lineare verso l'alto: da 5 (1.0x) a 9 (4.0x)
                    return 1.0f + (v - 5) * 0.75f;
                }
                else
                {
                    // Scala lineare verso il basso: da 5 (1.0x) a 1 (0.25x)
                    return 1.0f - (5 - v) * 0.1875f;
                }
                //return Mathf.Lerp(1f, 40f, (float)v / (float)MouseSpeedOptionMax) / 40f;
            }
        }

        public float Volume
        {
            get
            {
                var v = PlayerPrefs.GetInt("Volume", VolumeOptionDefault);
                //return Mathf.Log10((float)v / 10000f) * 20f;
                return Mathf.Log10((float)v / 10f) * 80f; // From 1 to 10

            }
        }

        public bool VerticalMouse
        {
            get
            {
                var v = PlayerPrefs.GetInt("VerticalMouse", VerticalMouseOptionDefault);
                return v != 0;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            // Load options

        }
        // Start is called before the first frame update
        void Start()
        {
            UpdateMixerVolume();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void UpdateMixerVolume()
        {
            mixer.SetFloat("Volume", Volume);
        }

        public void SaveOptions()
        {
            PlayerPrefs.Save();
            UpdateMixerVolume();
            OnOptionsChanged?.Invoke();
        }

        

    }
}
