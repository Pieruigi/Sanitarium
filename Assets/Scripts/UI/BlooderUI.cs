using System;
using UnityEngine;
using UnityEngine.UI;

namespace Baloon.UI
{


    public class BlooderUI : MonoBehaviour
    {
        [SerializeField]
        Sprite emptyBlooder;

        [SerializeField]
        Image blooderImage;

        [SerializeField]
        BlooderController controller;

        private void Awake()
        {
          
            BlooderController.OnSealed += HandleOnSealed;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {
            BlooderController.OnSealed -= HandleOnSealed;
        }

        private void HandleOnSealed(BlooderController blooderController)
        {
          

            if (controller != blooderController) return;

            // Switch sprite
            blooderImage.sprite = emptyBlooder;
        }
    }
}