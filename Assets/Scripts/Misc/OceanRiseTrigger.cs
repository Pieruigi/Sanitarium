using UnityEngine;

namespace Baloon
{
    public class OceanRiseTrigger : MonoBehaviour
    {
        [SerializeField]
        BloodOcean ocean;

        bool triggered = false;

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
            if (!other.CompareTag("Player") || triggered) return;

            triggered = true;

            ocean.Rise();
        }
    }
}