using StarterAssets;
using System.Collections;
using UnityEngine;

namespace Baloon
{
    public class CarrionFlyController : MonoBehaviour
    {
        [SerializeField] private FlyBobbing flyBobbing;
        [SerializeField] private Animator animator;
        [SerializeField] private float jumpscareTime = .2f;
        private float jumpScareSpeed = 15f; // Fast enough to startle the player

        private bool isJumpscaring = false;
        private Vector3 targetPosition;

        GameObject player;

        void Start()
        {
            if (flyBobbing != null) flyBobbing.Play();
            player = GameObject.FindGameObjectWithTag("Player");
        }

        void Update()
        {
#if UNITY_EDITOR
           
            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    // Trigger the animation and start the movement
            //    //if (animator != null) animator.SetBool("Flying", true);
                

            //    StartCoroutine(Hit());

               

                

            //    AudioManager.Instance.PlayJumpscare();
            //}

            //if (isJumpscaring)
            //{
            //    // Move the fly towards the camera frame by frame
            //    transform.position = Vector3.MoveTowards(transform.position, targetPosition, jumpScareSpeed * Time.deltaTime);

            //    // Optional: Make the fly look at the player while attacking
            //    transform.LookAt(targetPosition);

            //    // Stop moving if it reaches the camera (or very close)
            //    if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            //    {
            //        isJumpscaring = false;
            //    }
            //}
#endif
        }

        IEnumerator Hit()
        {
            animator.SetTrigger("Attack");

            yield return new WaitForSeconds(.25f);
            // Set the target exactly at the camera position
            targetPosition = player.transform.position + player.transform.forward * 1.5f + Vector3.up * 1f;

            isJumpscaring = true;

            // Stop the idle bobbing to prevent weird offsets during the dash
            if (flyBobbing != null) flyBobbing.enabled = false;

            var dist = Vector3.Distance(targetPosition, transform.position);
            jumpScareSpeed = dist / jumpscareTime;
           

            CameraShake.Instance.PlayJumpscare();
            //animator.SetBool("Flying", false);
            //animator.SetTrigger("Attack");
        }
    }
}