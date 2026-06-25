using UnityEngine;

namespace Baloon
{
    public class ChonchonTrigger : MonoBehaviour
    {
        [SerializeField]
        Transform target;

        [SerializeField]
        GameObject prefab;

        bool spawned = false;

        GameObject creature;

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
            if (spawned) return;

            spawned = true;
            creature = Instantiate(prefab, target);
        }
    }
}