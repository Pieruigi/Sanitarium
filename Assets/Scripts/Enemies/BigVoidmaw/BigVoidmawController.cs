using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class BigVoidmawController : MonoBehaviour
    {

        [SerializeField]
        AudioSource hitAudioSource;

        [SerializeField]
        List<AudioClip> hitAudioClips;

        [SerializeField]
        AudioSource releaseAudioSource;

        GameObject balloon;

        [SerializeField]
        Vector3 offset = new Vector3(0, -2.53999996f, -0.519999981f);

        [SerializeField]
        Transform head;

        bool attached = false;

        //string attachParam = "Attached";

        Rigidbody balloonRB;

        float lerpSpeed = 50;

        float snappedAngle;

        GameObject player;

        float dragDownSpeed = 2;

        [SerializeField]
        List<BigVoidmawTentacle> tentacles;

        private void Awake()
        {
            balloon = BaloonController.Instance.gameObject;// GameObject.FindGameObjectWithTag("Baloon");
            balloonRB = balloon.GetComponent<Rigidbody>();

            // Set initial position
            var pos = balloon.transform.position + offset + Vector3.down * 10f;
            var rot = balloon.transform.rotation;

            transform.position = pos;
            transform.rotation = rot;

            player = GameObject.FindGameObjectWithTag("Player");
            var playerFwd = player.transform.forward;
            playerFwd.y = 0;

            var bFwd = balloon.transform.forward;
            bFwd.y = 0;

            float angle = Vector3.SignedAngle(bFwd, -playerFwd, Vector3.up);

            snappedAngle = Mathf.Round(angle / 45f) * 45f;

            transform.rotation = rot * Quaternion.Euler(0f, snappedAngle, 0f);


            //// Animation
            //foreach (var t in tentacles)
            //{
            //    t.Play("Attack", 0, .7f);
                
            //}

            attached = true;

            // Jump scare
            CameraShake.Instance.PlayJumpscare(1f);

        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //foreach (Animator anim in tentacles)
            //{
            //    anim.SetBool(attachParam, true);
            //}

          
        }

        // Update is called once per frame
        void Update()
        {

        }

        

        private void LateUpdate()
        {
            if (attached)
            {
                Follow();
                FollowHead();

                // Drag down
                DragDown();
            }
        }

        private void DragDown()
        {
            var vel = dragDownSpeed * Time.deltaTime;
            balloon.transform.Translate(Vector3.down * vel);
            
        }

        private void FollowHead()
        {
            var target = Camera.main;

            head.LookAt(target.transform);
        }

        void Follow()
        {
            var pos = balloon.transform.position + offset;
            var rot = balloon.transform.rotation * Quaternion.Euler(0f, snappedAngle, 0f); ;// * Quaternion.Euler(0f, 22.5f, 0f); 
            transform.position = Vector3.Lerp(transform.position, pos, lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, lerpSpeed * Time.deltaTime);   
        }

        public void ReportTentacleHit()
        {
            // Audio
            hitAudioSource.clip = hitAudioClips[Random.Range(0, hitAudioClips.Count)];
            hitAudioSource.Play();

            foreach (var t in tentacles)
                if (t.IsAttached) return;

            attached = false;
            GetComponent<Rigidbody>().isKinematic = false;

            releaseAudioSource.Play();
        }

        /// <summary>
        /// Call it if you want to force the creature to release the balloon (for example if you are too close to a station).
        /// </summary>
        public void ForceDetach()
        {
            foreach (var t in tentacles)
                t.ForceDetach();

            attached = false;
            GetComponent<Rigidbody>().isKinematic = false;

            releaseAudioSource.Play();
        }
    }
}