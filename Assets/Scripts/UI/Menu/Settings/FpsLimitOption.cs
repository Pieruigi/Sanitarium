using System.Collections.Generic;
using UnityEngine;

namespace Baloon.UI
{
    public class FpsLimitOption : OptionList
    {

        protected override void Awake()
        {
            base.Awake();

            var options = new List<string>(new string[] { "Off", "30", "60" });
            var s = PlayerPrefs.GetInt(SettingsManager.FpsLimitOptionParam, SettingsManager.FpsLimitOptionDefault);
            Init(options, s);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void ReportValueChanged(int value)
        {
            PlayerPrefs.SetInt(SettingsManager.FpsLimitOptionParam, value);
            SettingsManager.Instance.SaveOptions();
        }
    }
}