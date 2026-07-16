using UnityEngine;

namespace Baloon.Demo
{
    public class Demo_EndTrigger : MonoBehaviour
    {
        [SerializeField]
        BlooderController blooder1, blooder2;

        private void Awake()
        {
#if !DEMO
            Destroy(gameObject);    
#endif

        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
#if DEMO
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Baloon")) return;
            
            if (!blooder1.Sealed || !blooder2.Sealed) return;

            GameManager.Instance.DemoEnd = true;

            KillerWind.Instance.PlayLargeTentacleKilling();
        }
#endif
    }
}