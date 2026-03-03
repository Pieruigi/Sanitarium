using UnityEngine;

namespace Baloon
{
    public class ExternalAir : Singleton<ExternalAir>
    {
        float celsius = 25f;
        public float Celsius => celsius;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}