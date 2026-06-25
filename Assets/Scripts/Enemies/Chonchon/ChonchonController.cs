using StarterAssets;
using UnityEngine;

namespace Baloon
{
    public class ChonchonController : MonoBehaviour
    {
        
        float targetY;

        FirstPersonController player;
        
        private void Awake()
        {
            player = FindFirstObjectByType<FirstPersonController>();
            targetY = player.transform.position.y + 2.5f;
        }

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