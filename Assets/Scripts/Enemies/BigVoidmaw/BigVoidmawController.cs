using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class BigVoidmawController : MonoBehaviour
    {

        [SerializeField]
        List<Animator> tentacles;

        GameObject balloon;

        [SerializeField]
        Vector3 offset = new Vector3(0, -2.53999996f, -0.519999981f);

        bool attached = false;

        string attachParam = "Attached";

        Rigidbody balloonRB;

        float lerpSpeed = 50;

        float snappedAngle;

        private void Awake()
        {
            balloon = GameObject.FindGameObjectWithTag("Baloon");
            balloonRB = balloon.GetComponent<Rigidbody>();

            // Set initial position
            var pos = balloon.transform.position + offset + Vector3.down * 10f;
            var rot = balloon.transform.rotation;

            transform.position = pos;
            transform.rotation = rot;

            var player = GameObject.FindGameObjectWithTag("Player");
            var playerFwd = player.transform.forward;
            playerFwd.y = 0;

            var bFwd = balloon.transform.forward;
            bFwd.y = 0;

            float angle = Vector3.SignedAngle(bFwd, -playerFwd, Vector3.up);

            snappedAngle = Mathf.Round(angle / 45f) * 45f;

            transform.rotation = rot * Quaternion.Euler(0f, snappedAngle, 0f);


            // Animation
            foreach (var t in tentacles)
            {
                t.Play("Attack", 0, .7f);
                
            }

            attached = true;

            // Jump scare
            CameraShake.Instance.PlayJumpscare(1f);

        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            foreach (Animator anim in tentacles)
            {
                anim.SetBool(attachParam, true);
            }
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
            }
        }

        void Follow()
        {
            var pos = balloon.transform.position + offset;
            var rot = balloon.transform.rotation * Quaternion.Euler(0f, snappedAngle, 0f); ;// * Quaternion.Euler(0f, 22.5f, 0f); 
            transform.position = Vector3.Lerp(transform.position, pos, lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, lerpSpeed * Time.deltaTime);   
        }
    }
}