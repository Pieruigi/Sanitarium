using UnityEngine;

namespace Baloon.UI
{
    public class VolumeOption : OptionSlider
    {

        protected override void Awake()
        {
            base.Awake();

            
        }

        private void Start()
        {
            Init(PlayerPrefs.GetInt(SettingsManager.VolumeOptionParam, SettingsManager.VolumeOptionDefault));
        }

        protected override void ReportValueChanged(float value)
        {
            PlayerPrefs.SetInt(SettingsManager.VolumeOptionParam,(int)value);
            SettingsManager.Instance.SaveOptions();
        }
    }
}