// [2026-05-22]
// Custom texture offset mover with Perlin Noise distortion for organic liquid movement.
using UnityEngine;

namespace Baloon
{
    public class MaterialOffsetMover : MonoBehaviour
    {
        [Header("Base Scroll Settings")]
        [SerializeField] private Vector2 baseScrollSpeed = new Vector2(0.01f, 0.01f);
        [SerializeField] private string texturePropertyName = "_MainTex";

        [Header("Noise Distortion Settings")]
        [SerializeField] private bool useNoise = true;
        [SerializeField] private float noiseStrength = 0.05f;
        [SerializeField] private float noiseFrequency = 0.5f;

        private Renderer meshRenderer;
        private Vector2 currentOffset = Vector2.zero;
        private float noiseTimer = 0f;

        // [2026-03-16]
        // Cache the renderer component at startup to optimize performance.
        void Start()
        {
            meshRenderer = GetComponent<Renderer>();

            if (meshRenderer == null)
            {
                Debug.LogError($"[MaterialOffsetMover] Missing Renderer on {gameObject.name}! Script disabled.");
                enabled = false;
            }
        }

        void Update()
        {
            // 1. Calculate the standard linear movement
            currentOffset += baseScrollSpeed * Time.deltaTime;

            Vector2 finalOffset = currentOffset;

            // 2. Apply Perlin Noise distortion if enabled to break the linear direction
            if (useNoise)
            {
                noiseTimer += Time.deltaTime * noiseFrequency;

                // Generate two different noise values for X and Y using coordinates offsets
                float noiseX = Mathf.PerlinNoise(noiseTimer, 0f) * 2.0f - 1.0f;
                float noiseY = Mathf.PerlinNoise(0f, noiseTimer) * 2.0f - 1.0f;

                // Add the wavy distortion to the final offset
                finalOffset.x += noiseX * noiseStrength;
                finalOffset.y += noiseY * noiseStrength;
            }

            // 3. Keep values within the 0-1 range to prevent precision loss over time
            finalOffset.x %= 1.0f;
            finalOffset.y %= 1.0f;

            // 4. Apply the distorted offset to the shader property
            meshRenderer.material.SetTextureOffset(texturePropertyName, finalOffset);
        }
    }
}