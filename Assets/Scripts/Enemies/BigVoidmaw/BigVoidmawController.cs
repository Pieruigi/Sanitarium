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

        private void Awake()
        {
            balloon = GameObject.FindGameObjectWithTag("Baloon");
            balloonRB = balloon.GetComponent<Rigidbody>();

            // Set initial position
            Follow();

            transform.position += Vector3.down * 10f;
            

            attached = true;
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
            var pos = balloonRB.position + offset;
            var rot = balloon.transform.rotation;// * Quaternion.Euler(0f, 22.5f, 0f); 
            transform.position = Vector3.Lerp(transform.position, pos, lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, lerpSpeed * Time.deltaTime);   
        }
    }
}