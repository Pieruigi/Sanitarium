using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Baloon
{
    public class VoidmawController : MonoBehaviour
    {
        [SerializeField]
        Animator animator;

        [SerializeField]
        Interactor interactor;

        [SerializeField]
        AudioSource hitAudioSource;

        [SerializeField]
        List<AudioClip> hitAudioClips;

        [SerializeField]
        AudioSource goreHitAudioSource;

        [SerializeField]
        List<AudioClip> goreHitAudioClips;

        [SerializeField]
        AudioSource hurtAudioSource;

        [SerializeField]
        List<AudioClip> hurtAudioClips;


        [SerializeField]
        AudioSource goreAudioSource;

        [SerializeField]
        AudioSource gruntOutAudioSource;

        [SerializeField]
        AudioSource metalOutAudioSource;

        BaloonControlPanel panel;

        FirstPersonController player;

        Rigidbody rb;

        bool fleeing = false;
        bool grabbing = false;

        //bool hitting = false;
        //float hitElapsed = 0;
        //float hitTime = 1f;
        int hitCount = 3;

     

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            //// Ignore balloon hit collision
            //var coll = GetComponent<Collider>();
            //var others = FindObjectsByType<BalloonCollisionChecker>(FindObjectsSortMode.None);
            //foreach (var other in others)
            //    Physics.IgnoreCollision(coll, other.GetComponent<Collider>(), true);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
     
            // We set the panel as parent
            panel = FindFirstObjectByType<BaloonControlPanel>();
            transform.parent = panel.transform;
            //transform.rotation = panel.transform.rotation;

            // We must set the position depending on the player position to avoid the creature to spawn from nowhere in front of the player 
            player = FindFirstObjectByType<FirstPersonController>();
            var pos = player.transform.position - Vector3.ProjectOnPlane(player.transform.forward, Vector3.up) * 1f + Vector3.up * 2f;
            transform.position = pos;

            // Move to the 
            var target = new Vector3(0.0255126953f, 1.48500001f, 0.110717773f);


            transform.DOLocalMove(target, .2f).OnComplete(() => 
            {
                grabbing = true;
                animator.SetTrigger("Grab"); 
                panel.DisableControls(); 
                
            });

            // Sound
            //goreHitAudioSource.clip = goreHitAudioClips[Random.Range(0, goreHitAudioClips.Count)];
            goreHitAudioSource.PlayDelayed(.1f);

            // Play loop
            goreAudioSource.PlayDelayed(.3f);

            // Play jumpscare
            CameraShake.Instance.PlayJumpscare();
        }

        // Update is called once per frame
        void Update()
        {
            if (!fleeing)
            {
                transform.localRotation = Quaternion.identity;
            }


            //if (grabbing && hitting)
            //{
            //    hitElapsed += Time.deltaTime;
            //    if(hitElapsed > hitTime)
            //    {
            //        hitElapsed -= hitTime;
            //        //Hit();
            //    }
            //}

        }

        private void OnEnable()
        {
            Interactor.OnInteractionStarted += HandleOnInteractionStarted;
            Interactor.OnInteractionStopped += HandleOnInteractionStopped;
            RepairToolEventListener.OnHit += HandleOnHit;
        }

        private void OnDisable()
        {
            Interactor.OnInteractionStarted -= HandleOnInteractionStarted;
            Interactor.OnInteractionStopped -= HandleOnInteractionStopped;
            RepairToolEventListener.OnHit -= HandleOnHit;
        }

        private void HandleOnHit()
        {
            Hit();
        }

        private void HandleOnInteractionStarted(Interactor interactor)
        {
            if (interactor != this.interactor || !grabbing || fleeing || !RepairToolController.Instance.Equipped) return;

            RepairToolController.Instance.StartRepairAnimation();

            //hitting = true;
            //hitElapsed = 0;
        }

        private void HandleOnInteractionStopped(Interactor interactor)
        {
            if (interactor != this.interactor) return;
            RepairToolController.Instance.StopRepairAnimation();
            //hitting = false;
        }

        void Hit()
        {
            if (!grabbing || fleeing) return;

            CameraShake.Instance.PlayWrenchHit();

            hitCount--;
            animator.SetTrigger("Hit");
            if (hitCount <= 0)
                StartCoroutine(Flee());

            // Gore audio
            if(hitCount > 0)
            {
                hurtAudioSource.clip = hurtAudioClips[Random.Range(0, hurtAudioClips.Count)];
                hurtAudioSource.Play();
            }
            else
            {
                gruntOutAudioSource.Play();
                metalOutAudioSource.Play();
            }


            // Play audio
            hitAudioSource.clip = hitAudioClips[Random.Range(0, hitAudioClips.Count)];
            hitAudioSource.Play();

            IEnumerator Flee()
            {
                fleeing = true;
                grabbing = false;

                yield return new WaitForSeconds(.1f);

                // Reset parenting
                transform.parent = null;

                // release controller
                panel.EnableControls();

                // Activate rigidbody
                rb.isKinematic = false;
                rb.useGravity = true;

                // Direction
                var dir = transform.forward + transform.up * 4f;
                rb.linearVelocity = BaloonController.Instance.CurrentVelocity;
                rb.AddForce(dir * 1.5f, ForceMode.VelocityChange);
            }
        }
    }
}