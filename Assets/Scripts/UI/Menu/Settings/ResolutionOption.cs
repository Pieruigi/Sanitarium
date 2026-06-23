using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Baloon.UI
{
    public class ResolutionOption : OptionList
    {
        string resolutionFormatString = "{0}x{1}";

        protected override void Awake()
        {
            base.Awake();
            InitOptionList();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }


        void InitOptionList()
        {
            var tmp = Screen.resolutions.ToList();
            List<string> options = new List<string>();

            foreach (var res in tmp)
            {
                int w = res.width;
                int h = res.height;
                string s = string.Format(resolutionFormatString, w, h);

                if (!options.Exists(d => d.Equals(s)))
                    options.Add(s);

            }

            // Get current resolution 
            var currRes = string.Format(resolutionFormatString, Screen.currentResolution.width, Screen.currentResolution.height);
            int index = options.IndexOf(currRes);
            if (index < 0)
                index = 0;

            Init(options, index);
        }

        protected override void ReportValueChanged(int value)
        {
            var refreshRate = Screen.currentResolution.refreshRateRatio;
            var option = GetOption(value);

            string[] optionSplit = option.Split("x");

            Screen.SetResolution(int.Parse(optionSplit[0]), int.Parse(optionSplit[1]), Screen.fullScreen); // ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed);
        }
    }
}