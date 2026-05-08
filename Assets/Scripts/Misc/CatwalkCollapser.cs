using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;


namespace Baloon
{
    public class CatwalkCollapser : MonoBehaviour
    {
        [SerializeField]
        float duration = 26f;

        [SerializeField]
        float inTime = 2f; 

        [SerializeField]
        float outTime = 1f;

        [SerializeField]
        List<Rigidbody> rigidbodies;

        [SerializeField]
        AudioSource audioSource;

        private void Awake()
        {
            foreach(var rigidbody in rigidbodies)
                rigidbody.isKinematic = true;
            
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR

            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    Play();
            //}
#endif
        }

        void Play()
        {
            // Start camera shake
            CameraShake.Instance.PlayCatwalkCollapseShake(duration);

            // Start destroying the catwalk
            StartCoroutine(DoCollapse());

            IEnumerator DoCollapse()
            {
                audioSource.Play();

                var time = duration - inTime - outTime;

                yield return new WaitForSeconds(inTime);

                float step = time / (float)rigidbodies.Count;
                int count = 0;

                do
                {
                    yield return new WaitForSeconds(step);

                    Debug.Log("TEST - Collapse rigidbody");
                    var r = rigidbodies[count];
                    r.GetComponent<MeshCollider>().convex = true;
                    r.isKinematic = false;


                    count++;

                }
                while (count < rigidbodies.Count);
                
                yield return new WaitForSeconds(outTime);

                audioSource.Stop();

            }
        }
    }


}