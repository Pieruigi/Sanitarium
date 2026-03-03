using TMPro;
using UnityEngine;

namespace Baloon.UI
{
    public class DebugUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text boilerPower;

        [SerializeField]
        TMP_Text temperature;

        [SerializeField]
        TMP_Text altitude;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            boilerPower.text = $"{BoilerController.Instance.Power.ToString("0.0")}";
            temperature.text = $"{InternalAir.Instance.TemperatureDifference.ToString("00.0")}";
            altitude.text = $"{InternalAir.Instance.transform.position.y.ToString("000.0")}";
        }
    }
    
}