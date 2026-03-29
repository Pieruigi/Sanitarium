using UnityEngine;
using UnityEngine.Events;

namespace Baloon
{
    public class RepairToolEventListener : MonoBehaviour
    {
        public static UnityAction OnHit;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Hit()
        {
            OnHit?.Invoke();
        }
    }
}