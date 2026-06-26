using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class ChonchonController : MonoBehaviour
    {
        public delegate void SpawnedDelegate(ChonchonController chonchon);
        public static SpawnedDelegate OnSpawned;
        
        enum State { Idle, Enter, Chasing, Leaving, BringingPlayer, Exit, Stunned }

        float targetY;

        FirstPersonController player;

        bool idle = false;

        State state;

        float chaseSpeed = .725f;

        Transform idleTarget;

        float chasingDelay = 2f;

        [SerializeField]
        AudioSource stunnedAudioSource;

        Animator animator;

        
        
        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();

            player = FindFirstObjectByType<FirstPersonController>();
            targetY = player.transform.position.y + 6f;

            // Adjust height
            transform.DOMoveY(targetY, 2f);

            // Set chasing state
            state = State.Enter;
            StartCoroutine(StartChasing());

            OnSpawned?.Invoke(this);

            IEnumerator StartChasing()
            {
                yield return new WaitForSeconds(chasingDelay);

                state = State.Chasing;
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if(Input.GetKeyDown(KeyCode.X))
                SetStunnedState();
#endif
        }

        private void LateUpdate()
        {
            switch (state)
            {
                case State.Idle:
                    UpdateIdleState();
                    break;
                case State.Enter:
                    LookAt(player.transform);
                    break;
                case State.Chasing:
                    UpdateChasingState();
                    break;
                case State.Exit:
                    UpdateExitState();
                    break;
            }
        }

        void UpdateIdleState()
        {
            LookAt(player.transform);

            // Reach idle target
            var targetPos = idleTarget.position;
            targetPos.y = transform.position.y;
            var newPos = Vector3.Lerp(transform.position, targetPos, chaseSpeed * Time.deltaTime);
            transform.position = newPos;

        }

        void UpdateExitState()
        {
            UpdateIdleState();

            // check distance
            if(Vector3.ProjectOnPlane(transform.position-idleTarget.position, Vector3.up).magnitude < 13f)
                GameObject.Destroy(gameObject);
        }

        void UpdateChasingState()
        {
            // Look at the palyer
            LookAt(player.transform);

            // Reach the player
            var targetPos = player.transform.position;
            targetPos.y = transform.position.y;
            var newPos = Vector3.Lerp(transform.position, targetPos, chaseSpeed * Time.deltaTime);
            transform.position = newPos;

            // Check distance
            var dist = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).magnitude;
            if(dist < 7f)
            {
                state = State.BringingPlayer;
                state = State.BringingPlayer;
                // Kill the player
                player.Doomed = true;
                player.MoveDisabled = true;

                //player.DisableAndLookForSeconds(transform.position, 10f);
            }
        }

        void LookAt(Transform target)
        {
            var dir = target.position - transform.position;
            dir = dir.normalized;
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        public void SetIdleState()
        {
            state = State.Idle;
        }

        public void UnsetIdleState()
        {
            StartCoroutine(DoUnsetIdle());

            IEnumerator DoUnsetIdle()
            {
                yield return new WaitForSeconds(chasingDelay);
                state = State.Chasing;
            }

            
        }

        public void SetIdleTarget(Transform target)
        {
            idleTarget = target;
        }

        public void SetExitState()
        {
            chaseSpeed = .25f;
            state = State.Exit;
        }

        public void SetStunnedState()
        {
            if (state == State.Stunned || state == State.BringingPlayer) return;

            state = State.Stunned;

            animator.SetBool("Stunned", true);

            stunnedAudioSource.Play();

            StartCoroutine(SetChasingStateDelayed(4.5f));

            IEnumerator SetChasingStateDelayed(float time)
            {
                yield return new WaitForSeconds(time);

                state = State.Chasing;
                animator.SetBool("Stunned", false);
            }
        }
    }
}