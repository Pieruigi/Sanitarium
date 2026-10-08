using Baloon.SaveSystem;
using Cinemachine;
using NUnit.Framework;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Baloon
{
    public class ImbuncheController : MonoBehaviour
    {
        
        [SerializeField]
        List<Transform> patrolPoints;

        [SerializeField]
        Animator animator;

        [SerializeField]
        Transform cameraTarget;

        [SerializeField]
        AudioSource goreAudioSource;

        [SerializeField]
        AudioSource footstepsAudioSource;

        /// <summary>
        /// 0: idle
        /// 1: walk
        /// 2: attack
        /// </summary>
        int state = 0;

        float idleTime = 1f;
        float currentIdleTime;

        float elapsed = 0;

        NavMeshAgent agent;

        int patrolIndex = 0;


        FirstPersonController player;

        string saveId = "imbunche";

        class Data
        {
            public bool activated;
        }

        private void Awake()
        {
            currentIdleTime = GetNewIdleTime();
            elapsed = 0;
            agent = GetComponent<NavMeshAgent>();
            //patrolIndex = 0;
            //transform.position = patrolPoints[patrolIndex].position;
            //transform.rotation = patrolPoints[patrolIndex].rotation;
            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
            Debug.Log("TEST - Imbunche awakened");
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = FindFirstObjectByType<FirstPersonController>();

            //state = 1;

            //agent.SetDestination(patrolPoints[patrolIndex].position);
            //// Set animation
            //animator.SetTrigger("Walk");
            // Save data
            string rawData = SaveManager.Instance.GetRawJsonData(saveId);
            var activated = false;
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData);
                activated = data.activated;
                Debug.Log("TEST - Imbunche data:" + activated);
            }

            
            gameObject.SetActive(activated);
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case 0:
                    UpdateIdleState();
                    break;
                case 1:
                    UpdateWalkState();
                    break;
                case 2:
                    UpdateAttackState();
                    break;

            }
        }

        private void OnEnable()
        {
            
            state = 1;

            agent.SetDestination(patrolPoints[patrolIndex].position);
            // Set animation
            animator.SetTrigger("Walk");

            // Play audio
            footstepsAudioSource.Play();
        }

        private void OnDestroy()
        {
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
        }

        private void HandleOnUpdateDataEntry()
        {
            var data = new Data();
            data.activated = gameObject.activeSelf;
            Debug.Log("TEST - Imbunche save:" + data.activated);
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        private void UpdateAttackState()
        {
            
        }

        private void UpdateWalkState()
        {
            if (CheckPlayerDistance())
            {
                AttackPlayer();
                return;
            }

            // Check target distance
            var distance = Vector3.Distance(transform.position, agent.destination);
            if(distance < .1f)
            {
                // Idle
                currentIdleTime = GetNewIdleTime();
                elapsed = 0;

                // Reset agent
                agent.ResetPath();

                // Play animation
                animator.SetTrigger("Idle");

                // Change state
                state = 0;

                // Stop audio
                footstepsAudioSource.Stop();

            }
        }

        private void UpdateIdleState()
        {
            if(CheckPlayerDistance())
            {
                AttackPlayer();
                return;
            }

            // Wait for few seconds
            elapsed += Time.deltaTime;
            if (elapsed < currentIdleTime) return;

            // Get the next patrol point
            patrolIndex++;
            if (patrolIndex >= patrolPoints.Count) patrolIndex = 0;
            agent.SetDestination(patrolPoints[patrolIndex].position);

            // Set animation
            animator.SetTrigger("Walk");

            // Change state
            state = 1;

            // Play audio
            footstepsAudioSource.Play();
        }

        float GetNewIdleTime()
        {
            //return Random.Range(idleTime * .9f, idleTime * 1.1f);
            return idleTime;
        }

        bool CheckPlayerDistance()
        {
            var dist = Vector3.Distance(player.transform.position, transform.position);
            
            return dist < 5f;

        }

        void AttackPlayer()
        {
            agent.ResetPath();
            agent.isStopped = true;
            state = 2;

            StartCoroutine(DoKill());

            IEnumerator DoKill()
            {
                player.Doomed = true;
                player.JawDisabled = true;
                player.PitchDisabled = true;
                player.MoveDisabled = true;

                // Disable flashlight
                Flashlight.Instance.gameObject.SetActive(false);

                //animator.Play("Attack", 0, 1);
                animator.SetTrigger("Attack");

                // Stop audio
                footstepsAudioSource.Stop();

                // Set camera parent
                //Camera.main.GetComponent<CinemachineBrain>().enabled = false;
                player.CinemachineCameraTarget.transform.parent = cameraTarget;
                player.CinemachineCameraTarget.transform.localPosition = Vector3.zero;
                player.CinemachineCameraTarget.transform.localRotation = Quaternion.identity;

                // Jumpscare
                CameraShake.Instance.PlayJumpscare(1.5f);

                // Play jumspcare audio
                AudioManager.Instance.PlayJumpscare();

                // Play gore delayed
                goreAudioSource.PlayDelayed(1.5f);

                yield return new WaitForSeconds(1.5f);

                player.Die(PlayerDeadType.CreatureAttack);

                yield break;
            }
        }

        public void SetPatroPoint(int index)
        {
            patrolIndex = index;
        }
    }
}