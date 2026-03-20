using StarterAssets;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Baloon
{
    public class BaloonController : Singleton<BaloonController>
    {
        public float Altitude => transform.position.y;

        //Rigidbody rb;

        float verticalForce = 5f;
        //[SerializeField]
        float horizontalForce = 0; // For accelerating (6 to reach speed 3)
        public float HorizontalForce
        {
            get { return horizontalForce; }
            set 
            {
                horizontalForce = value; 
                if (horizontalForce == 0)  currentVelocity.x = currentVelocity.z = 0f;  
            }
        }

        float maxVerticalSpeed = 6f;
        float maxHorizontalSpeed = 3; 

        [SerializeField] float gravity = 9.81f;
        [SerializeField] float linearDrag = 0.5f; // Simula l'attrito dell'aria

        [SerializeField] float groundCheckDistance = 1.5f; // Altezza della cesta
        [SerializeField] LayerMask groundLayer;

        //float verticalSpeed = 0f;
        Vector3 currentVelocity = Vector3.zero;
        public Vector3 CurrentVelocity => currentVelocity;

        Vector2 horizontalDirection = Vector2.zero;
        public Vector2 HorizontalDirection
        {
            get { return horizontalDirection; }
            set { horizontalDirection = value.normalized; }
        }
        
        GameObject player;
        CharacterController characterController;
        FirstPersonController firstPersonController;
        

        //bool useRB = false;

        protected override void Awake()
        {
            base.Awake();
            //rb = GetComponent<Rigidbody>();
            //if (!useRB) Destroy(rb);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            characterController = player.GetComponent<CharacterController>();
            firstPersonController = player.GetComponent<FirstPersonController>();
            Debug.Log($"TEST - playerCollider:{player.GetComponent<Collider>()}");
         
        }

       
        private void Update()
        {
            //if (useRB) return;
            
            UpdateVerticalVelocity();
            UpdateHorizontalVelocity();

            transform.position += currentVelocity * Time.deltaTime;

        }

        //private void FixedUpdate()
        //{
        //    if (!useRB) return;

        //    UpdateVerticalVelocityRB();
        //    UpdateHorizontalVelocityRB();
        //}

       

        void UpdateVerticalVelocity()
        {
            var diff = InternalAir.Instance.TemperatureDifference;

            //if (diff > 1.5 && diff < 2.5) diff = 2f;
            //diff = Mathf.Round(diff * 4f) / 4f;
            if (diff > 1.75f && diff < 2.25f)
                diff = 2f;

            float verticalSpeed = currentVelocity.y;

            // 1. CALCOLO ACCELERAZIONE (La tua logica originale)
            float acceleration = 0f;
            if (diff > 0)
            {
                float mul = 1f;
                if (verticalSpeed >= 0)
                {
                    mul = 1 - (verticalSpeed / maxVerticalSpeed);
                    mul = Mathf.Clamp01(mul);
                }
                // Spinta del bruciatore
                acceleration = diff * verticalForce * mul;
            }

            // 2. APPLICAZIONE GRAVITÀ E DRAG (Quello che faceva il Rigidbody)
            // Sottraiamo la gravità
            acceleration -= gravity;

            // Applichiamo l'accelerazione alla velocità
            verticalSpeed += acceleration * Time.deltaTime;

            // Applichiamo il Drag (l'attrito aumenta con la velocità)
            verticalSpeed *= (1f - linearDrag * Time.deltaTime);


            // Controllo del suolo
            if (verticalSpeed < 0) // Controlliamo solo se stiamo scendendo
            {
                RaycastHit hit;
                float startOffset = 1f;
                if (Physics.Raycast(transform.position + Vector3.up * startOffset, Vector3.down, out hit, groundCheckDistance + startOffset, groundLayer))
                {
                    // Se tocchiamo il suolo, azzeriamo la velocità e posizioniamo la cesta esattamente sopra
                    verticalSpeed = 0;

                    // Opzionale: corregge la posizione per non farla compenetrare
                    Vector3 pos = transform.position;
                    pos.y = hit.point.y + groundCheckDistance;
                    transform.position = pos;
                }
            }

            currentVelocity.y = verticalSpeed;

            // 3. MOVIMENTO FINALE
            // Muoviamo il transform direttamente (niente scatti per lo slider!)
            //transform.position += Vector3.up * currentVelocity.y * Time.deltaTime;
            
        }

        void UpdateHorizontalVelocity()
        {
            if (horizontalForce == 0) return;

            //horizontalDirection = Vector3.forward;
            // 1. Calculate acceleration (F = m * a, assuming mass = 1)
            // We start with the base force applied to the balloon
            Debug.Log("TEST - horizontal direction:" + horizontalDirection);
            Vector3 acceleration = new Vector3(horizontalDirection.x, 0f, horizontalDirection.y)  * horizontalForce;
            
            Vector3 horizontalVelocity = currentVelocity;
            horizontalVelocity.y = acceleration.y = 0f;
           

            // 2. Apply Linear Drag (Air Resistance)
            // Resistance increases proportionally to current velocity
            acceleration -= horizontalVelocity * linearDrag;
            

            // 3. Integrate acceleration into velocity (v = a * dt)
            horizontalVelocity += acceleration * Time.deltaTime;

            // 4. Safety Clamp (The physics math naturally stabilizes, but this is a fail-safe)
            if (horizontalVelocity.magnitude > maxHorizontalSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxHorizontalSpeed;
            }

           
            currentVelocity.x = horizontalVelocity.x;
            currentVelocity.z = horizontalVelocity.z;
           
            // 5. Update Transform position (p = v * dt)
            //transform.position += new Vector3(currentVelocity.x, 0f, currentVelocity.z) * Time.deltaTime;
        }

        //void UpdateVerticalVelocityRB()
        //{
        //    var diff = InternalAir.Instance.TemperatureDifference;

        //    if (diff > 0)
        //    {
        //        var mul = 1f;
        //        if (rb.linearVelocity.y >= 0)
        //        {
        //            mul = 1 - (rb.linearVelocity.y / maxVerticalSpeed);
        //            mul = Mathf.Clamp(mul, 0, 1);
        //        }
        //        rb.AddForce(Vector3.up * diff * verticalForce * mul, ForceMode.Acceleration);



        //    }
        //}

        //void UpdateHorizontalVelocityRB()
        //{

        //}

        
    }
}