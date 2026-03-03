using System.Collections.Generic;
using UnityEngine;

namespace SNT
{
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField]
        List<Transform> spots;

        [SerializeField]
        Transform target;

        TriggerButton trigger;

        int currentSpotIndex = 0;

        private void Awake()
        {
            trigger = GetComponent<TriggerButton>();
            target.position = spots[currentSpotIndex].position;
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
            TriggerButton.OnTriggered += SwitchCamera;
        }

        private void OnDisable()
        {
            TriggerButton.OnTriggered -= SwitchCamera;
        }

        void SwitchCamera(TriggerButton trigger)
        {
            if (trigger == this.trigger)
            {
                //GetComponent<Camera>().enabled = !GetComponent<Camera>().enabled;
                currentSpotIndex++;
                if(currentSpotIndex >= spots.Count)
                    currentSpotIndex = 0;
                target.position = spots[currentSpotIndex].position;
            }
        }
    }

}
