using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Baloon.UI
{


    public abstract class OptionList : MonoBehaviour
    {
        [SerializeField]
        Button prevButton, nextButton;

        [SerializeField]
        TMP_Text valueField;

        List<string> options = new List<string>();

        int currentIndex = 0;

        protected virtual void Awake()
        {
            nextButton.onClick.AddListener(MoveNext);
            prevButton.onClick.AddListener(MovePrev);
        }

        protected void Init(List<string> options, int currentIndex = 0)
        {
            this.options = options;
            this.currentIndex = currentIndex;

            valueField.text = options[currentIndex].ToString();

            CheckButtons();
        }

        void MovePrev()
        {
            currentIndex--;
            valueField.text = options[currentIndex].ToString();
            CheckButtons();
        }

        void MoveNext()
        {
            currentIndex++;
            valueField.text = options[currentIndex].ToString();
            CheckButtons();
        }

        void CheckButtons()
        {
            prevButton.interactable = currentIndex > 0 ? true : false;
            nextButton.interactable = currentIndex < options.Count - 1 ? true : false;
        }
    }
}