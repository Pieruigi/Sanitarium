using UnityEngine;

namespace Baloon.Demo
{
    public class DemoLogoText : MonoBehaviour
    {
        private void Awake()
        {
#if !DEMO
            Destroy(gameObject);
#endif

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