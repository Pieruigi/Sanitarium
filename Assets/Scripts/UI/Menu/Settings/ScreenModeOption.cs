using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon.UI
{
    public class ScreenModeOption : OptionList
    {
        protected override void Awake()
        {
            base.Awake();

            var options = new List<string>(new string[] { "windowed", "fullscreen" });
            Init(options, Screen.fullScreen ? 1 : 0);
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
            throw new System.NotImplementedException();
        }
    }
}