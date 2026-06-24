using System.Collections.Generic;
using UnityEngine;

namespace Baloon.UI
{
    public class VerticalMouse : OptionList
    {
        protected override void Awake()
        {
            base.Awake();

            var options = new List<string>(new string[] { "normal", "inverted" });
            var s = PlayerPrefs.GetInt(SettingsManager.VerticalMouseOptionParam, SettingsManager.VerticalMouseOptionDefault);
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
            PlayerPrefs.SetInt(SettingsManager.VerticalMouseOptionParam, value);
            SettingsManager.Instance.SaveOptions();
        }
    }
}