using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;

namespace Baloon
{
    public class BaloonController : Singleton<BaloonController>
    {
        Rigidbody rb;

        float force = 5f;

        float maxSpeed = 6f;

        GameObject player;

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            Debug.Log($"TEST - playerCollider:{player.GetComponent<Collider>()}");
         
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            var diff = InternalAir.Instance.TemperatureDifference;

            if(diff > 0)
            {
                var mul = 1f;
                if(rb.linearVelocity.y >= 0)
                {
                    mul = 1 - (rb.linearVelocity.y / maxSpeed);
                    mul = Mathf.Clamp(mul, 0, 1);
                }
                rb.AddForce(Vector3.up * diff * force * mul, ForceMode.Acceleration);


                //rb.AddTorque(Vector3.up * diff * force * 0.1f, ForceMode.Acceleration);
            }
                

            //if(rb.linearVelocity.y > 0 && rb.linearVelocity.y > maxSpeed)
            //    rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxSpeed, rb.linearVelocity.z);

        }
    }
}