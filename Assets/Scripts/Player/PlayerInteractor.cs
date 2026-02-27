using SNT.Interfaces;
using UnityEngine;

namespace SNT
{
    public class PlayerInteractor : Singleton<PlayerInteractor>
    {
        public delegate void MouseEnterDelegate(IInteractable interactable);
        public static MouseEnterDelegate OnMouseEnter;

        public delegate void MouseExitDelegate(IInteractable interactable);
        public static MouseExitDelegate OnMouseExit;

        

        float interactDistance = 3f;

        IInteractable interactable;

        override protected void Awake()
        {
            base.Awake();

            enabled = false;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (IsBusy()) return; // For example when you hit a triger button and it's not interactable for a bit


            if (Input.GetMouseButton(0) && interactable == null) return;
                        

            if(Input.GetMouseButtonUp(0) && interactable != null)
            {
                interactable.StopInteraction();
                return;
            }

            // Check input
            IInteractable newInteractable = null;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactDistance, LayerMask.GetMask(new string[] { "Interactable" })))
            {
                
                newInteractable = hit.collider.gameObject.GetComponent<IInteractable>();
            }

            if(newInteractable == null)
            {
                if (interactable != null)
                {
                    OnMouseExit?.Invoke(interactable);
                    interactable = null;
                }
            }
            else
            {
                if(interactable != null)
                {
                    OnMouseExit?.Invoke(interactable);
                    interactable = null;
                }
                interactable = newInteractable;
                OnMouseEnter?.Invoke(interactable);
            }

            if(interactable != null && interactable.IsInteractable())
            {
                if(Input.GetMouseButtonDown(0))
                {
                    interactable.StartInteraction();
                }
             
            }

        }

        private void OnEnable()
        {
            Debug.Log("TEST - Interactor enabled");
        }

        private void OnDisable()
        {
            Debug.Log("TEST - Interactor disabled");
        }

        public bool IsBusy()
        {
            return interactable != null && !interactable.IsInteractable();
        }
        
    }
}