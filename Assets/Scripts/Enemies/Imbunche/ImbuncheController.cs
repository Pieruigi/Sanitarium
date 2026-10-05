using NUnit.Framework;
using StarterAssets;
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

        /// <summary>
        /// 0: idle
        /// 1: walk
        /// 2: attack
        /// </summary>
        int state = 0;

        float idleTime = 3f;
        float currentIdleTime;

        float elapsed = 0;

        NavMeshAgent agent;

        int patrolIndex = 0;

        FirstPersonController player;

        private void Awake()
        {
            currentIdleTime = GetNewIdleTime();
            elapsed = 0;
            agent = GetComponent<NavMeshAgent>();
            patrolIndex = 0;
            transform.position = patrolPoints[patrolIndex].position;
            transform.rotation = patrolPoints[patrolIndex].rotation;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = FindFirstObjectByType<FirstPersonController>();
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
        }

        float GetNewIdleTime()
        {
            return Random.Range(idleTime * .9f, idleTime * 1.1f);
        }

        bool CheckPlayerDistance()
        {
            var dist = Vector3.Distance(player.transform.position, transform.position);
            
            return dist < 2f;

        }

        void AttackPlayer()
        {
            agent.ResetPath();
            agent.isStopped = true;
            state = 2;
        }
    }
}