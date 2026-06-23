using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Baloon.UI
{


    public abstract class OptionList : MonoBehaviour
    {
        public delegate void ValueChangedDelegate(int oldValue, int newValue);
        public ValueChangedDelegate OnValueChanged;

        [SerializeField]
        Button prevButton, nextButton;

        [SerializeField]
        TMP_Text valueField;

        List<string> options = new List<string>();

        int currentIndex = 0;

        LocalizeStringEvent localizeStringEvent;

        protected abstract void ReportValueChanged(int value);

        protected virtual void Awake()
        {
            nextButton.onClick.AddListener(MoveNext);
            prevButton.onClick.AddListener(MovePrev);

            localizeStringEvent = valueField.GetComponent<LocalizeStringEvent>();
        }

        protected void Init(List<string> options, int currentIndex = 0)
        {
            this.options = options;
            this.currentIndex = currentIndex;

            //valueField.text = options[currentIndex].ToString();
            UpdateValue();

            CheckButtons();
        }

        void MovePrev()
        {
            currentIndex--;
            //valueField.text = options[currentIndex].ToString();
            UpdateValue();
            CheckButtons();
            ReportValueChanged(currentIndex);
        }

        void MoveNext()
        {
            currentIndex++;
            //valueField.text = options[currentIndex].ToString();
            UpdateValue();
            CheckButtons();
            ReportValueChanged(currentIndex);
        }

        void CheckButtons()
        {
            prevButton.interactable = currentIndex > 0 ? true : false;
            nextButton.interactable = currentIndex < options.Count - 1 ? true : false;
        }

        public string GetOption(int index)
        {
            return options[index].ToString();
        }

        void UpdateValue()
        {
            if(!localizeStringEvent)
                valueField.text = options[currentIndex].ToString();
            else
                localizeStringEvent.StringReference.TableEntryReference = options[currentIndex];
        }
    }
}