using UnityEngine;
using UnityEngine.UI;

namespace Baloon.UI
{
    public abstract class OptionSlider : MonoBehaviour
    {
        [SerializeReference]
        Slider slider;

        protected abstract void ReportValueChanged(float value);

        protected virtual void Awake()
        {
            slider.onValueChanged.AddListener(ReportValueChanged);    
        }

        public void Init(float value)
        {
            slider.value = value;
        }
    }
}