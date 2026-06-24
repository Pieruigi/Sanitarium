using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Baloon.UI
{
    public class LanguageOption : OptionList
    {
        [System.Serializable]
        struct LocaleData
        {
            public string code;
            public string nativeName;   
        }

        List<LocaleData> data = new List<LocaleData>();

        protected override void Awake()
        {
            base.Awake();

            var availables = LocalizationSettings.AvailableLocales.Locales;
            foreach (var available in availables)
            {
                var d = new LocaleData();
                d.code = available.Identifier.Code;
                d.nativeName = available.Identifier.CultureInfo.NativeName;
                data.Add(d);
            }

            // Current locale
            var currentIndex = data.FindIndex(l=>l.code == LocalizationSettings.SelectedLocale.Identifier.Code);

            // Create list
            var options = new List<string>();
            foreach (var available in availables)
                options.Add(available.Identifier.CultureInfo.NativeName);

            Init(options, currentIndex);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("pt-br");
            if (Input.GetKeyDown(KeyCode.Alpha2))
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("es-ar");
            if (Input.GetKeyDown(KeyCode.Alpha3))
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("ja");
        }

        protected override void ReportValueChanged(int value)
        {
            // Get selected locale
            var selected = LocalizationSettings.AvailableLocales.Locales[value];
            LocalizationSettings.SelectedLocale = selected;
        }
    }
}