using UnityEngine;

namespace Baloon.Demo
{
    public class Demo_EndTrigger : MonoBehaviour
    {
        [SerializeField]
        BlooderController blooder1, blooder2;

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

            if (!blooder1.Sealed || !blooder2.Sealed) return;

            KillerWind.Instance.PlayLargeTentacleKilling();
        }
    }
}