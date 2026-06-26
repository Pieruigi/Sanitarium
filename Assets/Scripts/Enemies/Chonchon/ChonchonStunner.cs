using System;
using UnityEngine;

namespace Baloon
{
    public class ChonchonStunner : MonoBehaviour
    {
        [SerializeField]
        HoldButton button;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            button.OnPushed += HandleOnPushed;
        }

        private void OnDisable()
        {
            button.OnPushed -= HandleOnPushed;
        }

        private void HandleOnPushed()
        {
            // Get chonchon
            ChonchonController chonchon = FindFirstObjectByType<ChonchonController>();
            chonchon.SetStunnedState();
        }
    }
}