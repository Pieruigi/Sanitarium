using Baloon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SNT
{
    public class CameraSwitcher : MonoBehaviour
    {
        public static UnityAction OnSwitch;

        [SerializeField]
        List<Transform> spots;

        [SerializeField]
        HoldButton switchButton;

        int currentSpotIndex = 0;

        private void Awake()
        {
            transform.position = spots[currentSpotIndex].position;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            InitCamera();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            //TriggerButton.OnTriggered += SwitchCamera;
            switchButton.OnPushed += SwitchCamera;
        }

        private void OnDisable()
        {
            //TriggerButton.OnTriggered -= SwitchCamera;
            switchButton.OnPushed -= SwitchCamera;
        }

      

        void SwitchCamera()
        {
            Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
               
                //GetComponent<Camera>().enabled = !GetComponent<Camera>().enabled;
                currentSpotIndex++;
                if (currentSpotIndex >= spots.Count)
                    currentSpotIndex = 0;
                //target.position = spots[currentSpotIndex].position;
                InitCamera();

            OnSwitch?.Invoke();
            
        }

        void InitCamera()
        {
            var spot = spots[currentSpotIndex];
            var target = spot.GetChild(0);
            transform.parent = target;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

}
