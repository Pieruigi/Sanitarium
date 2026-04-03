using DG.Tweening;
using System.Collections; // To make the gust feel physical
using UnityEditor.ShaderGraph.Internal;
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

        [SerializeField] VerticalWindDirection direction;

        bool follow = false;

        bool triggered = false;


#if UNITY_EDITOR
        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.X))
        //    {
        //        ApplyPowerfulGust(BaloonController.Instance.transform, 1);
        //    }
        //}

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
            KillerWind.OnWarningStarted += HandleOnKillerWindWarningStarted;
            KillerWind.OnWarningStopped += HandleOnKillerWindWarningStopped;
        }

        private void OnDisable()
        {
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
            KillerWind.OnWarningStarted -= HandleOnKillerWindWarningStarted;
            KillerWind.OnWarningStopped -= HandleOnKillerWindWarningStopped;
        }

        private void HandleOnKillerWindWarningStarted()
        {
            HandleOnPathCleared();
        }

        private void HandleOnKillerWindWarningStopped()
        {
            HandleOnPathSet();
        }

        private void HandleOnPathSet()
        {
            if(BaloonPathManager.Instance.GetIndex(BaloonPathManager.Instance.CurrentPath) == pathIndex)
            {
                if (!triggered) follow = true;
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

            if (direction == VerticalWindDirection.Random)
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
                windDirection = direction == VerticalWindDirection.Up ? 1 : -1;
            }

            ApplyPowerfulGust(balloon, windDirection);
        }

        private void ApplyPowerfulGust(Transform target, int direction)
        {
            var duration = Random.Range(gustDuration * .9f, gustDuration * 1.1f);
            var strength = Random.Range(gustStrength * .9f, gustStrength * 1.1f);

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
                yield return new WaitForSeconds(duration * 1.5f);

                BaloonBoilerHealth.Instance.TakeSingleDamage();
            }
        }


    }
}