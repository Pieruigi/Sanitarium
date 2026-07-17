using TMPro;
using UnityEngine;

namespace Baloon.UI
{
    public class VersionUI : MonoBehaviour
    {
        private void Awake()
        {
            var field = GetComponent<TMP_Text>();
            field.text = "";
#if DEMO
            field.text = "Demo ";
#endif
            field.text += $"Version {Application.version}";
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}