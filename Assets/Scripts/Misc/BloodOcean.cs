using DG.Tweening;
using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Baloon
{
    public class BloodOcean : MonoBehaviour
    {
        [SerializeField]
        float minHeight;

        [SerializeField]
        float maxHeight;

        [SerializeField]
        AudioSource roarAudioSource;

        [SerializeField]
        AudioSource coughAudioSource;

        [SerializeField]
        AudioSource splashAudioSource;

        [SerializeField]
        AudioMixerGroup highFxMixerGroup;


        float step = 0;

        bool rising = false;

        float riseSpeed = 0;
        float riseAcc = .5f;
        bool dead = false;


        private void Awake()
        {
            var pos = transform.position;
            pos.y = minHeight;
            transform.position = pos;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.X))
            {
                var pos = transform.position;
                pos.y = -16f;
                transform.position = pos;
                Rise();


            }

#endif
        }

        private void LateUpdate()
        {
            if (!rising) return;

            // Rise ocean
            //riseSpeed += riseAcc * Time.deltaTime;
            //var pos = transform.position;
            //pos.y += riseSpeed * Time.deltaTime;
            //transform.position = pos;

            // Check altitude 
            if (!dead)
            {
                var camY = Camera.main.transform.position.y;
                if (Mathf.Abs(transform.position.y - camY) < 0.5f || camY < transform.position.y)
                {
                    dead = true;
                    var player = FindFirstObjectByType<FirstPersonController>();
                    player.Die(PlayerDeadType.GameEndingDrown);
                    
                    splashAudioSource.Play();
                }
            }
            
        }

        private void OnEnable()
        {
            BlooderController.OnSealed += HandleOnBlooderSealed;
            BlooderController.OnStarted += HandleOnBlooderStarted;
            BaloonPathManager.OnPathSet += HandleOnPathSet;
        }

        private void OnDisable()
        {
            BlooderController.OnSealed -= HandleOnBlooderSealed;
            BlooderController.OnStarted -= HandleOnBlooderStarted;
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
        }

        private void HandleOnPathSet()
        {
            var all = FindObjectsByType<BlooderController>(FindObjectsSortMode.None);
            foreach (var obj in all)
            {
                if (!obj.Sealed)
                    return;
            }
            // Increase altitude safe range
            //AltitudeManager.Instance.ForceGreen = true;
        }

        private void HandleOnBlooderStarted(BlooderController blooderController, bool isSealed)
        {
            if(step <= 0)
            {
                var count = FindObjectsByType<BlooderController>(FindObjectsSortMode.None).Length;
                step = (maxHeight - minHeight) / count;
            }

            if(isSealed)
            {
                var pos = transform.position;
                pos.y += step;
                transform.position = pos;
            }

            CheckForFinalStep();

        }

        private void HandleOnBlooderSealed(BlooderController blooderController)
        {
            var h = transform.position.y;
            h += step;
            transform.DOMoveY(h, 1f);

            CheckForFinalStep();
        }

        void CheckForFinalStep()
        {
            var all = FindObjectsByType<BlooderController>(FindObjectsSortMode.None);
            foreach (var obj in all)
            {
                if (!obj.Sealed)
                    return;
            }

            // Stop wind shake
            FindFirstObjectByType<WindShaker>().LightShakeOnly = true;
        }

        public void Rise()
        {
            
            StartCoroutine(DoRise());

            IEnumerator DoRise()
            {
                if (rising) yield break;

                var player = FindFirstObjectByType<FirstPersonController>();
                if (player.Doomed) yield break;

                player.Doomed = true;
                rising = true;

                

                // Stop wind
                FindFirstObjectByType<WindShaker>().Running = false;

                // Play some monster audio clip
                roarAudioSource.Play();

                // Engine cough
                //var h = coughAudioSource.transform.localPosition.y;
                //coughAudioSource.transform.parent = BaloonController.Instance.transform;
                //coughAudioSource.transform.localPosition = Vector3.up * h;
                //coughAudioSource.PlayDelayed(2f);


                // Shake camera
                CameraShake.Instance.PlayMoaningShake(8f);

                // Just wait 
                yield return new WaitForSeconds(5f);

                // Shut down boiler
                BaloonControlPanel.Instance.EngineStopAudioSource.outputAudioMixerGroup = highFxMixerGroup;
                BaloonControlPanel.Instance.Die();

                // Force balloon horizontal speed
                var maxSpeed = BaloonController.Instance.MaxHorizontalSpeed;
                var currentAltitude = BaloonController.Instance.Altitude;

                var targetAltitude = transform.position.y;

                var maxDistance = 25;

                float currentVerticalSpeed = BaloonController.Instance.CurrentVelocity.y; // Current variable vertical velocity (vy)
                float gravityG = 2.5f; // Gravitational acceleration (taking care of drag... more or less)

                // 1. Calculate the signed vertical distance to reach the target (targetAltitude is lower than currentAltitude)
                float verticalDisplacement = targetAltitude - currentAltitude; // This will be negative since we are going down

                // 2. Calculate time to reach target using the full kinematic equation:
                // s = v0 * t + 0.5 * a * t^2  ->  (0.5 * -g) * t^2 + v0 * t - verticalDisplacement = 0
                // Rearranged into standard quadratic form (a*t^2 + b*t + c = 0):
                // A = 0.5 * gravityG (acting downwards)
                // B = -currentVerticalSpeed
                // C = verticalDisplacement (negative)
                float a = 0.5f * gravityG;
                float b = -currentVerticalSpeed;
                float c = verticalDisplacement; // negative value

                float discriminant = (b * b) - (4f * a * c);

                float timeToReach = 0f;
                if (discriminant >= 0f)
                {
                    // Quadratic formula: t = (-b + sqrt(disc)) / (2*a)
                    timeToReach = (-b + Mathf.Sqrt(discriminant)) / (2f * a);
                }

                // Fallback safety check in case time is zero, negative, or NaN
                if (timeToReach <= 0f || float.IsNaN(timeToReach))
                {
                    timeToReach = 0.1f;
                }

                // 3. Calculate the required horizontal speed to cover maxDistance within that timeframe
                float newHorizontalSpeed = maxDistance / timeToReach;

                BaloonController.Instance.ForceMaxHorizontalSpeed(newHorizontalSpeed);
            }

            
            
        }
    }
}