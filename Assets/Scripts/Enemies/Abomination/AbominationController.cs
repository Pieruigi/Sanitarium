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

        bool chasingPlayer = false;

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

        void HandleOnTakeOff(BasePlatform basePlatform)
        {
            // I don't care witch platform since I register the event on abomination activation
            // Remove the callback
            BasePlatform.OnTakeOff -= HandleOnTakeOff;

            StartCoroutine(GoBack());

            IEnumerator GoBack()
            {
                yield return new WaitForSeconds(2f);

                StopChasingPlayer();
            }

            
        }

        public void StartChasingPlayer()
        {

            // Register platform event callback
            BasePlatform.OnTakeOff += HandleOnTakeOff;

            StartCoroutine(DoStartChasingPlayer());


            IEnumerator DoStartChasingPlayer()
            {
                // Start audio source
                audioSource.Play();

                animator.speed = 4f;

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