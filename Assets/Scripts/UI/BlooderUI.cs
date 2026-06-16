using System;
using UnityEngine;
using UnityEngine.UI;

namespace Baloon.UI
{


    public class BlooderUI : MonoBehaviour
    {
        [SerializeField]
        Material emptyBlooderMat;

        [SerializeField]
        Image blooderImage;

        [SerializeField]
        BlooderController controller;

        private void Awake()
        {
          
            //BlooderController.OnSealed += HandleOnSealed;
            //BlooderController.OnStarted += HandleOnBlooderStarted;
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
            //BlooderController.OnSealed -= HandleOnSealed;
            //BlooderController.OnStarted -= HandleOnBlooderStarted;
        }

        private void OnEnable()
        {
            if(controller.Sealed && blooderImage.material != emptyBlooderMat)
                blooderImage.material = emptyBlooderMat;
        }

        private void OnDisable()
        {
            
        }

        //private void HandleOnBlooderStarted(BlooderController blooderController, bool isSealed)
        //{
        //    Debug.Log($"TEST - Blooder started - {blooderController.transform.parent} - {isSealed}");
            

        //    if (controller != blooderController) return;

        //    Debug.Log($"TEST - UI - {controller.transform.parent}");

        //    if (isSealed)
        //        HandleOnSealed(blooderController);
        //}

        //private void HandleOnSealed(BlooderController blooderController)
        //{
          

        //    if (controller != blooderController) return;

        //    // Switch sprite
        //    blooderImage.material = emptyBlooderMat;
        //}
    }
}