using System;
using UnityEngine;


namespace SNT
{
    public class CameraOnOff : MonoBehaviour
    {
        [SerializeField]
        GameObject target;

        TriggerButton trigger;

        bool isOff = false;

        private void Awake()
        {
            trigger = GetComponent<TriggerButton>();
          
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
            TriggerButton.OnTriggered += SwitchOnOff;
        }

        private void OnDisable()
        {
            TriggerButton.OnTriggered -= SwitchOnOff;
        }

        private void SwitchOnOff(TriggerButton triggerButton)
        {
            if (triggerButton != this.trigger) return;
            
            isOff = !isOff;
            if(isOff)
            {
                target.GetComponent<Renderer>().material.color = Color.black;
            }
            else
            {
                target.GetComponent<Renderer>().material.color = Color.white;
            }

        }
    }
}