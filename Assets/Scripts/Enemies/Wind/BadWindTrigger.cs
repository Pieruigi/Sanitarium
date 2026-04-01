using UnityEngine;

namespace Baloon
{
    
    public class BadWindTrigger : MonoBehaviour
    {
        int level = 1;

        bool inside = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Baloon")) return;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Baloon")) return;
        }
    }
}