using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Baloon
{
    public class AbominationController : MonoBehaviour
    {
        [SerializeField]
        AudioSource audioSource;

        float targetTime = .5f;

        Transform target;

        NavMeshAgent agent;

        Vector3 initialPosition;

        Animator animator;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();  
            initialPosition = transform.position;
            
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            target = FindFirstObjectByType<FirstPersonController>().transform;
            animator = GetComponentInChildren<Animator>();
            animator.speed = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartChasingPlayer()
        {
            
            StartCoroutine(DoStartChasingPlayer());


            IEnumerator DoStartChasingPlayer()
            {
                // Start audio source
                audioSource.Play();

                animator.speed = 1;

                yield return new WaitForSeconds(.25f);

                
                

                while (true)
                {
                    agent.SetDestination(target.position);

                    yield return new WaitForSeconds(targetTime);
                }
            }

        }

        public void StopChasingPlayer()
        {
            // Stop the chasing coroutine
            StopAllCoroutines();

            StartCoroutine(DoStopChasingPlayer());

            IEnumerator DoStopChasingPlayer()
            {
                agent.SetDestination(initialPosition);

                yield return new WaitForSeconds(10f);

                Destroy(gameObject);
            }
        }
    }
}