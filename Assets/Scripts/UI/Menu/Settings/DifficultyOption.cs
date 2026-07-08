using System.Collections.Generic;
using UnityEngine;

namespace Baloon.UI
{
    public class DifficultyOption : OptionList
    {
        protected override void Awake()
        {
            base.Awake();

            
            
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var options = new List<string>(new string[] { "permadeath", "checkpoints" });
            Init(options, SettingsManager.Instance.GameMode);
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void ReportValueChanged(int value)
        {
            //GameManager.Instance.Difficulty = value;

            PlayerPrefs.SetInt(SettingsManager.GameModeOptionParam, (int)value);
            SettingsManager.Instance.SaveOptions();
        }
    }
}