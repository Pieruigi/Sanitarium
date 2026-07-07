using DG.Tweening;
using System.Collections; // To make the gust feel physical
using UnityEngine;

namespace Baloon
{
    public class BadWindTrigger : MonoBehaviour
    {
        enum VerticalWindDirection { Random, Up, Down}

        [Header("Gust Settings")]
        [SerializeField] private float gustStrength = 10f;
        [SerializeField] private float gustDuration = 1.5f;

        [SerializeField] int pathIndex = 0;

        [SerializeField] VerticalWindDirection windDirection;

        [SerializeField] BaloonPathDirection pathDirection;

        bool follow = false;

        bool triggered = false;

        private void Awake()
        {
            var r = GetComponent<Renderer>();
            if (r) r.enabled = false;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                ApplyPowerfulGust(BaloonController.Instance.transform, 1);
            }
        }

#endif

        private void LateUpdate()
        {
            if (!follow || triggered) return;

            var pos = transform.position;
            pos.y = BaloonController.Instance.transform.position.y;
            transform.position = pos;
        }

        private void OnEnable()
        {
            BaloonPathManager.OnPathSet += HandleOnPathSet;
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
            KillerWind.OnKilling += HandleOnKillerWindWarningStarted;
          
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
            KillerWind.OnKilling -= HandleOnKillerWindWarningStarted;
          
        }

        private void HandleOnKillerWindWarningStarted()
        {
            HandleOnPathCleared();
        }

        private void HandleOnPathSet()
        {
            if (BaloonPathManager.Instance.GetIndex(BaloonPathManager.Instance.CurrentPath) == pathIndex &&
               (pathDirection == BaloonPathDirection.Both || (pathDirection == BaloonPathDirection.Forward && !BaloonPathManager.Instance.IsPathReversed) || (pathDirection == BaloonPathDirection.Reversed && BaloonPathManager.Instance.IsPathReversed)))

            //if (BaloonPathManager.Instance.GetIndex(BaloonPathManager.Instance.CurrentPath) == pathIndex)
            {
                //if (!triggered) follow = true;
                triggered = false;
                follow = true;
            }
                    
        }

        private void HandleOnPathCleared()
        {

            follow = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Using your tag 'Baloon' as per your setup
            if (!other.CompareTag("Baloon")) return;
            if (!follow) return;
            if (triggered) return;

            triggered = true;

            var balloon = BaloonController.Instance.transform;
            var altitudeManager = AltitudeManager.Instance;

            int windDirection; // 1: Up, -1: Down
            var range = altitudeManager.GetCurrentRange();

            if (this.windDirection == VerticalWindDirection.Random)
            {
                if (range != AltitudeRange.Red)
                {
                    // Safe/Warning zone: random chaos
                    windDirection = Random.Range(0, 2) == 0 ? -1 : 1;
                }
                else
                {
                    // Red Zone: Punishment logic
                    // Compare current height with the target ideal height
                    if (balloon.position.y < altitudeManager.TargetAltitude)
                        windDirection = -1; // Already too low? Push further down!
                    else
                        windDirection = 1;  // Already too high? Push further up!
                }
            }
            else
            {
                windDirection = this.windDirection == VerticalWindDirection.Up ? 1 : -1;
            }

            ApplyPowerfulGust(balloon, windDirection);
        }

        float ComputeWindStrength(float currentAltitude)
        {
            const float minH = 20f;
            const float maxH = 120f;
            float minW = gustStrength;
            float maxW = gustStrength * 1.5f;
            // 1. Calculate the rate of change (Wind units per Meter)
            // In this case: (1.5 - 0.5) / (120 - 30) = 1.0 / 90 = ~0.011
            float windPerMeter = (maxW - minW) / (maxH - minH);

            // 2. Apply the slope starting from the base point (30m, 0.5W)
            // This works for 10m, 200m, or any value.
            var windStrength = minW + (currentAltitude - minH) * windPerMeter;

            // Optional: Safety check to avoid negative wind if altitude goes below zero
            if (windStrength < 0) windStrength = 0;

            return windStrength;
        }

        private void ApplyPowerfulGust(Transform target, int direction)
        {
            var duration = Random.Range(gustDuration * .9f, gustDuration * 1.1f);
            var strength = Random.Range(gustStrength * .9f, gustStrength * 1.1f);

            //strength = ComputeWindStrength(BaloonController.Instance.Altitude);
            strength = ComputeWindStrength(1f); // I don't the wind to change it's power depending on the altitude
            Debug.Log("TEST - Gust strength:" + strength);

            // Calculate displacement
            float targetY = target.position.y + (direction * strength);

            // Stop the wind shaker
            WindShaker.Instance.Running = false;

            // Stop the constante vertical wind
            VerticalWind.Instance.Running = false;

            // 2. Move the balloon violently (Ease.OutQuad starts fast and then slows down)
            target.DOMoveY(targetY, duration).SetEase(Ease.OutQuad);

            // Play camera shake
            CameraShake.Instance.PlayWindGustShake(duration, Reset, Reset);

            // Rotate baloon
            BaloonShaker.Instance.ShakeHeavyForWindGust(duration);

            // Apply audio
            WindAudio.Instance.FadeGustVolume(duration);

            StartCoroutine(ApplyDamage(duration));

            // 3. Audio Hook (Example)
            // AudioSource.PlayClipAtPoint(windGustClip, target.position);

            Debug.Log($"<color=cyan>[WindTrigger]</color> Gust applied! Direction: {direction}");

            void Reset()
            {
                WindShaker.Instance.Running = true;
                VerticalWind.Instance.Running = true;
            }

            IEnumerator ApplyDamage(float duration)
            {
                if (Random.Range(0, 10) < -3) yield break; // 70% we take damage

                yield return new WaitForSeconds(duration * 1.5f);

                if (BaloonBoilerHealth.Instance.TryTakeSingleDamage())
                {
                    CameraShake.Instance.PlayJumpscare(1f);

                    if (Random.Range(0, 4) == 0) // 25% we take a second more damage
                    {
                        // Double damage
                        yield return new WaitForSeconds(1f);

                        if(BaloonBoilerHealth.Instance.TryTakeSingleDamage())
                            CameraShake.Instance.PlayJumpscare(1f);
                    }
                }

                
            }
        }


    }
}