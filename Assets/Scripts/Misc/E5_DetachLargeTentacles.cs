using UnityEngine;

namespace Baloon
{
    public class E5_DetachLargeTentacles : MonoBehaviour
    {
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
            if (!other.CompareTag("Player")) return;

            var t = FindFirstObjectByType<E5_LargeTentacles>();
            t.ForceDetach();
        }
    }
}