using Unity.VisualScripting;
using UnityEngine;

namespace Baloon
{
    public class BaloonController : MonoBehaviour
    {
        Rigidbody rb;

        float force = 5f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            var diff = InternalAir.Instance.TemperatureDifference;

            if(diff > 0)
                rb.AddForce(Vector3.up * diff * force, ForceMode.Acceleration);
        }
    }
}