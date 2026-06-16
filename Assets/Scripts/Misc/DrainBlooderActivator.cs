using System;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class DrainBlooderActivator : MonoBehaviour
    {
        [SerializeField]
        BlooderController blooder;

        [SerializeField]
        List<GameObject> activateList;

        [SerializeField]
        List<GameObject> deactivateList;

        private void Awake()
        {
            foreach (GameObject go in activateList)
                go.SetActive(false);
            
        }


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
            BlooderController.OnSealed += HandleOnBlooderSelead;
            BlooderController.OnStarted += HandleOnBlooderStarted;
        }

        private void OnDisable()
        {
            BlooderController.OnSealed -= HandleOnBlooderSelead;
            BlooderController.OnStarted -= HandleOnBlooderStarted;
        }

        private void HandleOnBlooderStarted(BlooderController controller, bool isSealed)
        {
            if (blooder != controller) return;

            if(isSealed)
                HandleOnBlooderSelead(controller);
        }

        private void HandleOnBlooderSelead(BlooderController blooderController)
        {
            foreach (GameObject go in activateList)
                go.SetActive(true);
            
            foreach (GameObject go in deactivateList)
                go.SetActive(false);
        }
    }
}