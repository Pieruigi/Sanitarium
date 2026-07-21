using System.Collections;
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

#if !UNITY_WEBGL
            Initialize();
#endif
        }

#if UNITY_WEBGL
        IEnumerator Start()
        {
            //while(LocalizationSettings.AvailableLocales.Locales == null) yield return null;
            while (!LocalizationSettings.InitializationOperation.IsDone) yield return null;
            //while (LocalizationSettings.AvailableLocales.Locales.Count <= 1) yield return null;
            Debug.Log("TEST - Locales:" + LocalizationSettings.AvailableLocales.Locales.Count);
            Initialize();
        }
#endif

        // Update is called once per frame
        void Update()
        {
#if UNITY_WEBGL
            //Debug.Log("TEST - Locales:" + LocalizationSettings.AvailableLocales.Locales.Count);
#endif
            //if(Input.GetKeyDown(KeyCode.Alpha1))
            //    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("pt-br");
            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("es-ar");
            //if (Input.GetKeyDown(KeyCode.Alpha3))
            //    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("ja");
        }

        protected override void ReportValueChanged(int value)
        {
            // Get selected locale
            var selected = LocalizationSettings.AvailableLocales.Locales[value];
            LocalizationSettings.SelectedLocale = selected;
        }

        void Initialize()
        {
            var availables = LocalizationSettings.AvailableLocales.Locales;
            foreach (var available in availables)
            {
                var d = new LocaleData();
                d.code = available.Identifier.Code;
                d.nativeName = available.Identifier.CultureInfo.NativeName;
                data.Add(d);
            }

            // Current locale
            var currentIndex = data.FindIndex(l => l.code == LocalizationSettings.SelectedLocale.Identifier.Code);

            // Create list
            var options = new List<string>();
            foreach (var available in availables)
                options.Add(available.Identifier.CultureInfo.NativeName);

            Init(options, currentIndex);
        }
    }
}