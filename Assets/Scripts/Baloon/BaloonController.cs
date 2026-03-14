using StarterAssets;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Baloon
{
    public class BaloonController : Singleton<BaloonController>
    {
        public float Altitude => transform.position.y;

        Rigidbody rb;

        float force = 5f;

        float maxSpeed = 6f;

        [SerializeField] float gravity = 9.81f;
        [SerializeField] float linearDrag = 0.5f; // Simula l'attrito dell'aria

        [SerializeField] float groundCheckDistance = 1.5f; // Altezza della cesta
        [SerializeField] LayerMask groundLayer;

        float verticalVelocity = 0f;

        GameObject player;

        bool useRB = false;

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody>();
            if (!useRB) Destroy(rb);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            Debug.Log($"TEST - playerCollider:{player.GetComponent<Collider>()}");
         
        }

       
        private void Update()
        {
            if (useRB) return;
            var diff = InternalAir.Instance.TemperatureDifference;

            //if (diff > 1.5 && diff < 2.5) diff = 2f;
            //diff = Mathf.Round(diff * 4f) / 4f;
            if (diff > 1.75f && diff < 2.25f)
                diff = 2f;


            // 1. CALCOLO ACCELERAZIONE (La tua logica originale)
            float acceleration = 0f;
            if (diff > 0)
            {
                float mul = 1f;
                if (verticalVelocity >= 0)
                {
                    mul = 1 - (verticalVelocity / maxSpeed);
                    mul = Mathf.Clamp01(mul);
                }
                // Spinta del bruciatore
                acceleration = diff * force * mul;
            }

            // 2. APPLICAZIONE GRAVITÀ E DRAG (Quello che faceva il Rigidbody)
            // Sottraiamo la gravità
            acceleration -= gravity;

            // Applichiamo l'accelerazione alla velocità
            verticalVelocity += acceleration * Time.deltaTime;

            // Applichiamo il Drag (l'attrito aumenta con la velocità)
            verticalVelocity *= (1f - linearDrag * Time.deltaTime);


            // Controllo del suolo
            if (verticalVelocity < 0) // Controlliamo solo se stiamo scendendo
            {
                RaycastHit hit;
                float startOffset = 1f;
                if (Physics.Raycast(transform.position + Vector3.up * startOffset, Vector3.down, out hit, groundCheckDistance + startOffset, groundLayer))
                {
                    // Se tocchiamo il suolo, azzeriamo la velocità e posizioniamo la cesta esattamente sopra
                    verticalVelocity = 0;

                    // Opzionale: corregge la posizione per non farla compenetrare
                    Vector3 pos = transform.position;
                    pos.y = hit.point.y + groundCheckDistance;
                    transform.position = pos;
                }
            }


            // 3. MOVIMENTO FINALE
            // Muoviamo il transform direttamente (niente scatti per lo slider!)
            transform.position += Vector3.up * verticalVelocity * Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (!useRB) return;

            var diff = InternalAir.Instance.TemperatureDifference;

            if (diff > 0)
            {
                var mul = 1f;
                if (rb.linearVelocity.y >= 0)
                {
                    mul = 1 - (rb.linearVelocity.y / maxSpeed);
                    mul = Mathf.Clamp(mul, 0, 1);
                }
                rb.AddForce(Vector3.up * diff * force * mul, ForceMode.Acceleration);



            }
        }

       
    }
}