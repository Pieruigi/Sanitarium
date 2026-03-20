using UnityEngine;

namespace Baloon.UI
{
    public class CanvasCameraInitializer : MonoBehaviour
    {

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}