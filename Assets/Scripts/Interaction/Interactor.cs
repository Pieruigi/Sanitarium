using StarterAssets;
using System.ComponentModel;
using TMM;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Baloon
{
    public class Interactor : MonoBehaviour
    {
        public delegate void InteractionStartedDelegate(Interactor interactor);
        public static InteractionStartedDelegate OnInteractionStarted;

        public delegate void InteractionStoppedDelegate(Interactor interactor);
        public static InteractionStoppedDelegate OnInteractionStopped;

        public const float InteractionDistance = 10f;

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
        bool usePointer = false;

        bool inside = false;

        bool showMessage = false;

        bool interacting = false;

       
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            bool stopInteracting = false;
            if (inside)
            {
               
                RaycastHit hit;
                LayerMask mask = LayerMask.GetMask(new string[] { "Interactable" });

                Physics.SyncTransforms();

                var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                if(usePointer)
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

             
                if (Physics.Raycast(ray, out hit, InteractionDistance, mask))
                {
                   
                    if (hit.collider == interactionCollider)
                    {
                        
                        // Show message if any
                        showMessage = true;
                        // Check interaction
                        if ((mouseButton0 && Input.GetMouseButtonDown(0)) || Input.GetKeyDown(key))
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


                }
                else
                {
                    if (interacting && !stopOnKeyUpOnly)
                        stopInteracting = true;
                   
                }
            }

            if ((mouseButton0 && Input.GetMouseButtonUp(0)) || Input.GetKeyUp(key))
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