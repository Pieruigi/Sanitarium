using UnityEngine;

namespace Baloon
{
    public class LeftHand : Singleton<LeftHand>
    {
        public bool IsFree { get; set; }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            IsFree = true;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}