using Baloon.SaveSystem;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMM;
using UnityEngine;

namespace Baloon
{

    public class CreepyWormController : MonoBehaviour
    {
        [SerializeField]
        BlooderController blooder;

        [SerializeField]
        GameObject wormPrefab;

        [SerializeField]
        List<AudioClip> wormClips;

        [SerializeField]
        AudioClip crunchyClip;

        /// <summary>
        /// 0: nothing
        /// 10: blooder just sealed
        /// 20: path activated
        /// 30: completed
        /// </summary>
        int state = 0;

        string saveId = "f7_creepy_worm";

        GameObject worm;

        float flyingTime = 30;
        float flyingElapsed = 0;

        Collider lookCollider;

        public class Data
        {
            public int state = 0;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var rawData = SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData);
                state = data.state;

                switch (state)
                {
                    case 10:
                    case 20: // Just for the sake of clarity, we can't really save state 20 since we are flying
                        Spawn();
                        break;
                    default:
                        break;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    HandleOnSealed(blooder);
            //}
#endif


            switch (state)
            {
                case 20:
                    UpdateFlyingState();
                    break;
                default:
                    break;
            }
        }

        private void OnEnable()
        {
            BlooderController.OnSealed += HandleOnSealed;
            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
            BaloonPathManager.OnPathSet += HandleOnPathSet;
        }

        private void OnDisable()
        {
            BlooderController.OnSealed -= HandleOnSealed;
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
            BaloonPathManager.OnPathSet -= HandleOnPathSet;
        }

        private void HandleOnPathSet()
        {
            if (state != 10) return;

            state = 15;

            StartCoroutine(ChangeStateDelayed());

            IEnumerator ChangeStateDelayed()
            {
                yield return new WaitForSeconds(3f);

                state = 20;
            }
        }

        private void HandleOnUpdateDataEntry()
        {
            Data data = new Data();
            data.state = state;
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        private void HandleOnSealed(BlooderController blooderController)
        {
            if (blooderController != blooder) return;

            Spawn();
            state = 10;
        }

        void Spawn()
        {
            // Instantiate
            worm = Instantiate(wormPrefab, BaloonController.Instance.transform);
            worm.transform.localPosition = Vector3.zero;

            // Disable rigidbody
            worm.GetComponent<Rigidbody>().isKinematic = true;

            // Init the look collider
            lookCollider = worm.transform.Find("Sphere").GetComponent<Collider>();
            
            // Set direction
            var dir = blooder.GetComponentInChildren<ActivationTrigger>().transform.position - BaloonController.Instance.transform.position;
            dir.y = 0;
            worm.transform.forward = dir;

            // Play audio
            StartCoroutine(PlayWormAudio());

            IEnumerator PlayWormAudio()
            {
                var audioSource = worm.GetComponentInChildren<AudioSource>();
                
                while (state != 30)
                {
                    float time = Random.Range(1.8f, 2.2f);
                    
                    yield return new WaitForSeconds(time);

                    audioSource.clip = wormClips[Random.Range(0, wormClips.Count)];
                    audioSource.Play();
                }
            }
        }

        

        void UpdateFlyingState()
        {
            if (state != 20) return;

            bool fall = false;

            flyingElapsed += Time.deltaTime;
            if(flyingElapsed > flyingTime)
            {
                fall = true;
            }
            else
            {
                // Looking at?
                // Raycast
                RaycastHit hit;
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 7f))
                {
                    if (hit.collider == lookCollider)
                        fall = true;
                }
                
            }

            if (fall)
            {
                state = 25;

                StartCoroutine(FallDelayed());
            }

            IEnumerator FallDelayed()
            {
                yield return new WaitForSeconds(1f);

                state = 30;

                // Activate rigidbody
                var rb = worm.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                //rb.AddForce(Vector3.down * 15f, ForceMode.VelocityChange);

                // Change animation
                worm.GetComponentInChildren<Animator>().SetTrigger("Attack");

                // Play audio
                var audio = worm.GetComponentInChildren<AudioSource>();
                audio.clip = crunchyClip;
                audio.volume = 1f;
                audio.Play();

                // Destroy delayed
                Destroy(worm, 10f);
                
            }
        }
        
    }
}