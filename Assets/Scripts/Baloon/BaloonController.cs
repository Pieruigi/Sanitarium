using Baloon.SaveSystem;
using DG.Tweening;
using PSXShadersPro.URP.Demo;
using StarterAssets;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class BaloonController : Singleton<BaloonController>
    {
        /// <summary>
        /// 1.0 for 1.4 speed
        /// 1.286 for 1.8 speed
        /// 1.429 for 2.0 speed
        /// </summary>
        public const float SpeedMultiplier = 1.43f;

        [SerializeField]
        AudioSource landingAudioSource;

        [SerializeField]
        List<AudioClip> landingAudioClips;
                

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

                horizontalForce = value * horizontalForceScale;

#if UNITY_EDITOR
                //if (horizontalForce > 0) horizontalForce = 10;
#endif


                if (horizontalForce == 0)  currentVelocity.x = currentVelocity.z = 0f;  
            }
        }

        float horizontalForceScale = 9f;// 3f;

        float maxVerticalSpeed = 6f;
#if UNITY_EDITOR
        float maxHorizontalSpeed = 3.5f * 1.4f * SpeedMultiplier;// 1.4f;// 3.5f * 1.2f;
#else
        float maxHorizontalSpeed = 3.5f * 1.4f * SpeedMultiplier; 
#endif



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


        string saveId = "balloon";

        bool verticalVelocityDisabled = false;
        bool horizontalVelocityDisabled = false;

        class Data
        {
            public Vector3 position;
            public Quaternion rotation;
        }

        //bool useRB = false;

        protected override void Awake()
        {
            base.Awake();
            //rb = GetComponent<Rigidbody>();
            //if (!useRB) Destroy(rb);

#if DEMO
            //maxHorizontalSpeed = 3.5f * 1.4f;
#endif

#if UNITY_EDITOR
            //transform.position = new Vector3(240, 30, 605);
#endif
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            characterController = player.GetComponent<CharacterController>();
            firstPersonController = player.GetComponent<FirstPersonController>();
            
            // Save data
            string rawData = SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData);
                transform.position = data.position;
                transform.rotation = data.rotation;
            }
        }

       
        private void Update()
        {
            Physics.SyncTransforms();

            //if (useRB) return;

            float lastVelY = currentVelocity.y;


            UpdateVerticalVelocity();
            UpdateHorizontalVelocity();

            transform.position += currentVelocity * Time.deltaTime;

            CheckLandingAndTakeOff(lastVelY);

        }

        private void LateUpdate()
        {
           
        }

        private void OnEnable()
        {
            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
            BaloonPathManager.OnPathSet += HandleOnPathSet;
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
        }

        private void OnDisable()
        {
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
        }

        private void HandleOnPathSet()
        {
            
        }

        private void HandleOnPathCleared()
        {
            
        }

        private void HandleOnUpdateDataEntry()
        {
            var data = new Data();
            data.position = transform.position;
            data.rotation = transform.rotation;
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        //private void FixedUpdate()
        //{
        //    if (!useRB) return;

        //    UpdateVerticalVelocityRB();
        //    UpdateHorizontalVelocityRB();
        //}

        void CheckLandingAndTakeOff(float lastVelY)
        {
            if (BasePlatform.CurrentPlatform == null) return;

            if(lastVelY < 0 && currentVelocity.y == 0)
            {
                var min = -4.5f;
                var max = -2f;
                float power = Mathf.Lerp(0f, 1f, (max - lastVelY) / (max - min));
                CameraShake.Instance.PlayLandingShake(power);
                //BaloonCreek.Instance.Stop();
                //BaloonCreek.Instance.AdjustVolumeByFactor(0);

                var minVolume = .5f;
                var maxVolume = .8f;
                landingAudioSource.volume = Mathf.Lerp(minVolume, maxVolume, power);
                landingAudioSource.clip = landingAudioClips[Random.Range(0, landingAudioClips.Count)];
                landingAudioSource.Play();

                // Damage
                if (power > .75f)
                    BaloonBoilerHealth.Instance.TryTakeSingleDamage();
            }
            else if (lastVelY == 0 && currentVelocity.y > 0)
            {
                CameraShake.Instance.PlayTakeOffShake();
                BaloonCreek.Instance.Play(3, 0);
                //BaloonCreek.Instance.AdjustVolumeByFactor(0);
                //Debug.Log("TEST - VSpeed:"+currentVelocity.y);  
            }
            
        }

        void UpdateVerticalVelocity()
        {
            if(verticalVelocityDisabled) return;

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

//#if UNITY_EDITOR
//            if(acceleration > 0)
//            {
//                acceleration *= .1f;
//            }
//#endif

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
            if (horizontalForce == 0 || horizontalVelocityDisabled) return;

            //horizontalDirection = Vector3.forward;
            // 1. Calculate acceleration (F = m * a, assuming mass = 1)
            // We start with the base force applied to the balloon
            
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

            //Debug.Log($"TEST - Current speed:{new Vector2(currentVelocity.x, currentVelocity.z).magnitude}");
           
            // 5. Update Transform position (p = v * dt)
            //transform.position += new Vector3(currentVelocity.x, 0f, currentVelocity.z) * Time.deltaTime;
        }

        public void ResetHorizontalVelocity()
        {
            currentVelocity.x = currentVelocity.z = 0f;
        }

        public void SetHorizontalVelocity(Vector2 velocity)
        {
            currentVelocity.x = velocity.x;
            currentVelocity.z = velocity.y;
        }

        //public void SetVerticalVelocity()

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
        public void DisableVerticalVelocity()
        {
            currentVelocity.y = 0;
            verticalVelocityDisabled = true;
            VerticalWind.Instance.enabled = false;
        }

        public void DisableHorizontalVelocity()
        {
            currentVelocity.x = currentVelocity.z = 0;
            horizontalVelocityDisabled = true;
        }
        
    }
}