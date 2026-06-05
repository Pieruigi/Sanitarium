using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Baloon
{
    public class WindLimit : MonoBehaviour
    {
        [SerializeField]
        bool topLimit = false; // Is top or bottom limit?


        bool running = false;

        bool killing = false;

        bool inside = false;

        float offset;

        bool processing = false;

        private void Awake()
        {
            offset = GetComponent<BoxCollider>().size.y / 2f + 2f;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if (!running || killing) return;

            // Adjust altitude
            AdjustAltitude();
        }


        private void OnEnable()
        {
            BasePlatform.OnLanding += HandleOnLanding;
            BasePlatform.OnTakeOff += HandleOnTakeOff;
            //BaloonPathManager.OnPathSet += HandleOnPathSet;
            //BaloonPathManager.OnPathCleared += HandleOnPathCleared;
        }

        private void OnDisable()
        {
            BasePlatform.OnLanding -= HandleOnLanding;
            BasePlatform.OnTakeOff -= HandleOnTakeOff;
            //BaloonPathManager.OnPathSet -= HandleOnPathSet;
            //BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
        }

        private void HandleOnTakeOff(BasePlatform platform)
        {
            running = true;
        }

        private void HandleOnLanding(BasePlatform platform)
        {
            running = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            inside = true;

            if (processing) return;

            if (topLimit)
                ProcessTopLimit();
        }

        private void OnTriggerExit(Collider other)
        {
            inside = false;
        }


        void ProcessTopLimit()
        {
            processing = true;

            StartCoroutine(DoProcess());

            IEnumerator DoProcess()
            {
                while (inside)
                {
                                     
                    BaloonBoilerHealth.Instance.TryTakeSingleDamage();

                    yield return new WaitForSeconds(1.5f);
                }

                processing = false;
            }
        }

        void AdjustAltitude()
        {
            var min = AltitudeManager.Instance.MinAltitude;
            var max = AltitudeManager.Instance.MaxAltitude;

            float y = 0;
            if (topLimit)
            {
                y = max + (max - min) + offset;
            }
            else
            {
                y = min - (max - min) - offset;
            }

            var pos = transform.position;
            pos.y = y;
            transform.position = pos;
                
        }
    }
}