using UnityEngine;

namespace Baloon
{
    public class TerrainActivator : MonoBehaviour
    {
        [SerializeField]
        bool activate = false;

        [SerializeField]
        Terrain terrain;

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
            if(!other.CompareTag("Player")) return;

            if (activate)
            {
                if(!terrain.enabled) terrain.enabled = true;
            }
            else
            {
                if(terrain.enabled) terrain.enabled = false;
            }
        }
    }
}