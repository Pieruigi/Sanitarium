using UnityEngine;

namespace Baloon.UI
{
    public class MouseSpeedOption : OptionSlider
    {
        protected override void Awake()
        {
            base.Awake();

            Init(PlayerPrefs.GetInt(SettingsManager.MouseSpeedOptionParam, SettingsManager.MouseSpeedOptionDefault));
        }

        protected override void ReportValueChanged(float value)
        {
            PlayerPrefs.SetInt(SettingsManager.MouseSpeedOptionParam, (int)value);
            SettingsManager.Instance.SaveOptions();
        }
    }
}