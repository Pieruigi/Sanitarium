using UnityEngine;

namespace Baloon
{
    public class FlyBobbing : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float hoverSpeed = 2.0f;  // How fast it moves up and down
        [SerializeField] private float hoverAmount = 0.15f; // How far it moves (amplitude)

        private Vector3 startPos;
        private float randomOffset;

        bool running = false;

        void Start()
        {
            

            // Randomize the start point so multiple flies aren't perfectly synced
            randomOffset = Random.Range(0f, 100f);
        }

        void Update()
        {
            //if (!running) return;

            // Calculate the new Y position using a Sine wave
            // Formula: StartY + sin(Time * Speed + Offset) * Amount
            float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed + randomOffset) * hoverAmount;

            // Apply the new position while keeping X and Z the same
            transform.position = new Vector3(startPos.x, newY, startPos.z);
        }

        public void Play()
        {
            running = true;
            // Store the starting position to hover around it
            startPos = transform.position;
        }

        public void Stop()
        {
            running = false;
        }
    }
}