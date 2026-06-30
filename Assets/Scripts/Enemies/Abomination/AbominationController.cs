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

        [SerializeField]
        Light deadLight;

        [SerializeField]
        Transform deadTarget;

        float targetTime = .5f;

        Transform target;

        NavMeshAgent agent;

        Vector3 initialPosition;

        Animator animator;

        bool chasingPlayer = false;

        bool killingPlayer = false;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();  
            initialPosition = transform.position;
            deadLight.enabled = false;
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

        private void LateUpdate()
        {
            if (chasingPlayer)
            {
                // Check distance
                var dist = target.position - transform.position;
                if (dist.magnitude < 1.5f)
                {
                    StopAllCoroutines();
                    chasingPlayer = false;
                    killingPlayer = true;
                    agent.ResetPath();
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    KillPlayer();
                   
                }
            }
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
            chasingPlayer = true;

            // Register platform event callback
            BasePlatform.OnTakeOff += HandleOnTakeOff;

            StartCoroutine(DoStartChasingPlayer());


            IEnumerator DoStartChasingPlayer()
            {
                // Start audio source
                audioSource.Play();

                animator.speed = 4f;

                yield return new WaitForSeconds(.25f);

                while (chasingPlayer)
                {
                  
                    agent.SetDestination(target.position);

                    yield return new WaitForSeconds(targetTime);
                }
            }

        }

        void KillPlayer()
        {
            StartCoroutine(DoKill());

            IEnumerator DoKill()
            {
                // Stop player input and look the abomination direction
                FirstPersonController player = target.GetComponent<FirstPersonController>();
                player.Doomed = true; 
                player.JawDisabled = true;
                player.PitchDisabled = true;
                player.MoveDisabled = true;

                Debug.Log("TEST - Killing player");
                // Disable flashlight
                Flashlight.Instance.gameObject.SetActive(false);

                //player.DisableAndLookForSeconds(transform.position, 100f);

                // Force player position
                //var pos = transform.position + transform.forward * 2f;
                //player.ForcePosition(pos);

                // Enable abomination light
                deadLight.enabled = true;

                // Move the camera
                var cam = CameraShake.Instance.transform;
                cam.position = deadTarget.position;
                cam.rotation = deadTarget.rotation;

                // Stop animation
                animator.speed = 0;

                // Jumpscare
                CameraShake.Instance.PlayJumpscare(1.5f);

                yield return new WaitForSeconds(1.5f);

                player.Die(PlayerDeadType.CreatureAttack);


            }

            



        }

        public void StopChasingPlayer()
        {
            chasingPlayer = false;

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