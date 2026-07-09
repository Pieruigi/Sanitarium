using DG.Tweening;
using StarterAssets;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

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

        FirstPersonController player;

        float redTime = 10f;
        float redElapsed = 0;

        private void Awake()
        {
            offset = GetComponent<BoxCollider>().size.y / 2f + 2f;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = FindFirstObjectByType<FirstPersonController>();
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //    ProcessBottomLimit();
#endif

        }

        private void LateUpdate()
        {
            if (!running || killing) return;

            // Adjust altitude
            AdjustAltitude();

            if (!topLimit) return; // We only need one of the trigger
            if (player.Doomed || processing) return;
            var curRange = AltitudeManager.Instance.GetCurrentRange();
            if (curRange == AltitudeRange.Red)
            {
                redElapsed += Time.deltaTime;
                if (redElapsed > redTime)
                {
                    //if (!player.Doomed && !processing)
                    {
                        var middle = (AltitudeManager.Instance.MaxAltitude - AltitudeManager.Instance.MinAltitude) / 2f + AltitudeManager.Instance.MinAltitude;
                        

                        if (BaloonController.Instance.Altitude >  middle) ProcessBottomLimit();
                        else ProcessBottomLimit();
                    }
                    
                }
            }
            else
            {
                if(redElapsed > 0)
                {
                    redElapsed -= Time.deltaTime;
                    if (redElapsed < 0) redElapsed = 0;
                }
            }
            
            
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
            if (!other.CompareTag("Baloon")) return;

            inside = true;

            if (player.Doomed) return;

            if (processing) return;

            if (topLimit) ProcessBottomLimit();
            else ProcessBottomLimit();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Baloon")) return;

            inside = false;
        }

        private void ProcessBottomLimit()
        {
            processing = true;

            //KillerWind.Instance.StartKilling();
            KillerWind.Instance.PlayLargeTentacleKilling();

            //StartCoroutine(DoProcess());

            //IEnumerator DoProcess()
            //{
            //    float time = 4f;

            //    KillerWind.Instance.StartWarning(time);

            //    yield return new WaitForSeconds(time);

            //    if(inside)
            //        KillerWind.Instance.StartKilling();
            //    else
            //        KillerWind.Instance.StopWarning();
            //}
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