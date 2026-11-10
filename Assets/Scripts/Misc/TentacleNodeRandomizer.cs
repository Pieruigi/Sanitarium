using UnityEngine;


namespace Baloon
{
    using UnityEngine;

    /// <summary>
    /// Procedurally rotates a hierarchy chain of bones to create organic, idle tentacle movements.
    /// </summary>
    public class TentacleNodeRandomizer : MonoBehaviour
    {
        [Header("Bone Setup")]
        [Tooltip("Drag and drop the root (first) bone of the tentacle chain here.")]
        [SerializeField] private Transform rootBone;

        [Header("Movement Settings")]
        [Tooltip("Overall speed of the procedural sway motion.")]
        [SerializeField] private float speed = 1.0f;

        [Tooltip("Maximum rotation angle (in degrees) applied to the tip node.")]
        [SerializeField] private float maxRotationAngle = 15.0f;

        [Tooltip("Rotation intensity multiplier for the base bone (0 = base stays completely stationary).")]
        [Range(0f, 1f)]
        [SerializeField] private float baseMultiplier = 0.05f;

        // Internal data structure to cache initial transforms and noise offsets per bone
        private class BoneData
        {
            public Transform transform;
            public Quaternion initialLocalRotation;
            public Vector3 noiseOffset;
            public float weight; // Interpolated weight from 0 (root) to 1 (tip)
        }

        private BoneData[] bones;

        private void Start()
        {
            // Fallback to this GameObject's transform if no root bone is assigned
            if (rootBone == null)
            {
                rootBone = transform;
            }

            InitializeBoneChain();
        }

        private void InitializeBoneChain()
        {
            // Traverses and collects the single-chain bone hierarchy starting from root
            var boneList = new System.Collections.Generic.List<Transform>();
            Transform current = rootBone;

            while (current != null)
            {
                boneList.Add(current);
                // Assumes a linear bone chain (picks the first child)
                current = current.childCount > 0 ? current.GetChild(0) : null;
            }

            int count = boneList.Count;
            bones = new BoneData[count];

            for (int i = 0; i < count; i++)
            {
                // Calculate relative depth weight along the chain: 0 (root) to 1 (tip)
                float weight = count > 1 ? (float)i / (count - 1) : 1f;

                bones[i] = new BoneData
                {
                    transform = boneList[i],
                    initialLocalRotation = boneList[i].localRotation,
                    // Unique noise offsets prevent all nodes from swaying in unison
                    noiseOffset = new Vector3(
                        Random.Range(0f, 100f),
                        Random.Range(0f, 100f),
                        Random.Range(0f, 100f)
                    ),
                    weight = weight
                };
            }
        }

        private void LateUpdate()
        {
            if (bones == null || bones.Length == 0) return;

            float time = Time.time * speed;

            for (int i = 0; i < bones.Length; i++)
            {
                BoneData bone = bones[i];

                // Lerp max angle based on node depth (smaller range at base, maximum range at tip)
                float currentMaxAngle = Mathf.Lerp(maxRotationAngle * baseMultiplier, maxRotationAngle, bone.weight);

                // Generate smooth Perlin Noise values scaled to [-1, 1] range for each axis
                float yaw = (Mathf.PerlinNoise(time + bone.noiseOffset.x, 0f) - 0.5f) * 2f;
                float pitch = (Mathf.PerlinNoise(0f, time + bone.noiseOffset.y) - 0.5f) * 2f;
                float roll = (Mathf.PerlinNoise(time + bone.noiseOffset.z, time) - 0.5f) * 2f;

                // Calculate local offset rotation relative to default pose
                Quaternion offsetRotation = Quaternion.Euler(
                    pitch * currentMaxAngle,
                    yaw * currentMaxAngle,
                    roll * currentMaxAngle * 0.5f // Dampen roll for smoother organic sway
                );

                // Apply calculated offset on top of initial local bind pose
                bone.transform.localRotation = bone.initialLocalRotation * offsetRotation;
            }
        }
    }
}
