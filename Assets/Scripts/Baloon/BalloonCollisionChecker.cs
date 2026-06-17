using Mono.Cecil;
using StarterAssets;
using System.Linq;
using UnityEngine;


namespace Baloon
{
    public class BalloonCollisionChecker : MonoBehaviour
    {
        FirstPersonController player;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var collider = GetComponent<Collider>();
            var rb = collider.GetComponent<Rigidbody>();
            

            // Remove player collision
            player = GameObject.FindFirstObjectByType<FirstPersonController>();

            Physics.IgnoreCollision(collider, player.GetComponent<Collider>(), true);

            // Disable balloon internal collisions
            var root = GetComponentInParent<BaloonController>().gameObject;
            var others = root.GetComponentsInChildren<Collider>().Where(c=>c != collider);
            foreach (var other in others)
                Physics.IgnoreCollision(collider, other, true);

            // Disable platform collisions
            //var platforms = FindObjectsByType<BasePlatform>(FindObjectsSortMode.None);
            //foreach(var other in platforms)
            //    Physics.IgnoreCollision(collider, other.GetComponent<Collider>(), true);


        }

        // Update is called once per frame
        void Update()
        {

        }


        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("TEST - Collision with " + collision.gameObject);
            if (BasePlatform.CurrentPlatform || player.Doomed) return;

            StartCoroutine(GetComponentInParent<BaloonDestroyer>().DoPlayExplosion());
        }

    }
}