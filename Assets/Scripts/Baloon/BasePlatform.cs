using System;
using TMM;
using UnityEngine;

namespace Baloon
{
    public class BasePlatform : MonoBehaviour
    {
        public static BasePlatform CurrentPlatform { get; private set; }

        [SerializeField]
        ActivationTrigger trigger;

        bool inside;

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
            trigger.OnEnter += HandleOnEnter;
            trigger.OnExit += HandleOnExit;
        }

        private void OnDisable()
        {
            trigger.OnEnter -= HandleOnEnter;
            trigger.OnExit -= HandleOnExit;
        }

        private void HandleOnEnter(Collider other)
        {
            inside = true;
            CurrentPlatform = this;
        }

        private void HandleOnExit(Collider other)
        {
            inside = false;
            CurrentPlatform = null;
                
        }
    }
}