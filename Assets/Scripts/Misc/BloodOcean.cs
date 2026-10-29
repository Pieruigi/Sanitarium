using DG.Tweening;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Baloon
{
    public class BloodOcean : MonoBehaviour
    {
        [SerializeField]
        float minHeight;

        [SerializeField]
        float maxHeight;


        float step = 0;

        bool rising = false;

        float riseSpeed = 0;
        float riseAcc = 5f;


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
                // Increase altitude safe range
                AltitudeManager.Instance.AlwaysSafe = true;
                // Stop wind shake
                FindFirstObjectByType<WindShaker>().Running = false;
            }
            
#endif
        }

        private void LateUpdate()
        {
            if (!rising) return;

            riseSpeed += riseAcc * Time.deltaTime;
            var pos = transform.position;
            pos.y += riseSpeed * Time.deltaTime;
            transform.position = pos;
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
            AltitudeManager.Instance.AlwaysSafe = true;
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
            FindFirstObjectByType<WindShaker>().Running = false;
        }

        public void Rise()
        {
            if (rising) return;

            rising = true;

            
        }
    }
}