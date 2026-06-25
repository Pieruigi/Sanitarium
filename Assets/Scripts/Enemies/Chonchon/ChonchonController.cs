using DG.Tweening;
using StarterAssets;
using System.Collections;
using UnityEngine;

namespace Baloon
{
    public class ChonchonController : MonoBehaviour
    {
        public delegate void SpawnedDelegate(ChonchonController chonchon);
        public static SpawnedDelegate OnSpawned;
        
        enum State { Idle, Enter, Chasing, Leaving, BringingPlayer }

        float targetY;

        FirstPersonController player;

        bool idle = false;

        State state;

        float chaseSpeed = .25f;
        
        private void Awake()
        {
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
                yield return new WaitForSeconds(2f);

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

        }

        private void LateUpdate()
        {
            switch (state)
            {
                case State.Idle:
                case State.Enter:
                    UpdateIdleState();
                    break;
                case State.Chasing:
                    UpdateChasingState();
                    break;
            }
        }

        void UpdateIdleState()
        {
            LookAt(player.transform);
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
            state = State.Chasing;
        }
    }
}