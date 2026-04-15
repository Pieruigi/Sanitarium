using UnityEngine;

namespace Baloon
{
    public class DeathTrigger : MonoBehaviour
    {
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
            if (triggered) return;

            if (!other.CompareTag("Player")) return;

            if (BaloonPathManager.Instance.CurrentPath == null) return;

            triggered = true;

            KillerWind.Instance.StartKilling();
        }

        
    }
}