using StarterAssets;
using System;
using System.ComponentModel;
using TMM;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Baloon
{
    public class Interactor : MonoBehaviour
    {
        public delegate void HintDelegate(bool interactable);
        public static HintDelegate OnHint;

        public delegate void InteractionStartedDelegate(Interactor interactor);
        public static InteractionStartedDelegate OnInteractionStarted;

        public delegate void InteractionStoppedDelegate(Interactor interactor);
        public static InteractionStoppedDelegate OnInteractionStopped;

        public const float InteractionDistance = 1.5f;

        [SerializeField]
        ActivationTrigger activationTrigger;

        [SerializeField]
        Collider interactionCollider;

        [SerializeField]
        bool stopOnKeyUpOnly = false;

        [SerializeField]
        bool mouseButton0 = true;

        [SerializeField]
        KeyCode key = KeyCode.None;

        [SerializeField]
        InputActionReference inputAction;

        
        bool inside = false;

        bool showMessage = false;

        bool interacting = false;

        bool oldInputIsPressed = false;
        
        [SerializeField]
        bool _test = false;

        bool noHint;
        public bool NoHint
        {
            get { return noHint; }
            set { noHint = value; if (noHint) OnHint?.Invoke(false); }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var showMessageOld = showMessage;

            bool stopInteracting = false;

            var actionWasPressed = oldInputIsPressed;
            var actionIsPressed = false;
            if (inputAction)
            {
                actionIsPressed = inputAction.action.IsPressed();
                oldInputIsPressed = actionIsPressed;
            }

            if (inside)
            {
               
                
                RaycastHit hit;
                LayerMask mask = LayerMask.GetMask(new string[] { "Interactable" });

                
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, InteractionDistance, mask))
                {
                    
                    if (hit.collider == interactionCollider)
                    {
                        
                        // Show message if any
                        showMessage = true;
                        // Check interaction
                        if ((mouseButton0 && Input.GetMouseButtonDown(0)) || Input.GetKeyDown(key) || (actionIsPressed && !actionWasPressed))
                        {
                            interacting = true;
                            OnInteractionStarted?.Invoke(this);
                        }
                        //else if ((mouseButton0 && Input.GetMouseButtonUp(0)) || Input.GetKeyUp(key))
                        //{
                        //    if (interacting)
                        //        stopInteracting = true;
                            
                        //}


                    }
                    else
                    {
                        showMessage = false;
                    }

                }
                else
                {
                    showMessage = false;

                    if (interacting && !stopOnKeyUpOnly)
                        stopInteracting = true;
                   
                }
            }
            else
            {
                showMessage = false;
            }

            if ((mouseButton0 && Input.GetMouseButtonUp(0)) || Input.GetKeyUp(key) || (!actionIsPressed && actionWasPressed))
            {
                if (interacting)
                    stopInteracting = true;

            }

            if (stopInteracting)
            {
                stopInteracting = false;
                interacting = false;
                OnInteractionStopped?.Invoke(this);
            }

            if(showMessage != showMessageOld)
            {
                if (showMessage && !NoHint)
                    OnHint?.Invoke(true);
                else
                    OnHint?.Invoke(false);
            }
            
        }

        void OnEnable()
        {
            activationTrigger.OnEnter += HandleOnEnter;
            activationTrigger.OnExit += HandleOnExit;

           
        }

        void OnDisable()
        {
            activationTrigger.OnEnter -= HandleOnEnter;
            activationTrigger.OnExit -= HandleOnExit;

      
        }



        private void HandleOnEnter(Collider other)
        {
            inside = true;
           
        }

        private void HandleOnExit(Collider other)
        {
            inside = false;
           
        }

        

    }
}