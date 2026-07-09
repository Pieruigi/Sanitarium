using Baloon.UI;
using StarterAssets;
using UnityEngine;

namespace Baloon.Demo
{
    public class Demo_StationNotAvailable : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            var p = other.GetComponent<FirstPersonController>();
            if (!p.OnBaloon)
                LauncherDemoUI.Instance.ShowMessage();
        }
    }

}
