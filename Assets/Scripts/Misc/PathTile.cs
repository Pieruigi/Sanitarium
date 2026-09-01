using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Baloon
{

    public class PathTile : MonoBehaviour
    {
        [SerializeField]
        bool walkable = false;

        public bool Walkable => walkable;

        private void Awake()
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //float min = .5f;
            //float max = 1f;
            //transform.DOShakeRotation(1f, Random.Range(min, max), 4, 0f, false)
            //.SetLoops(-1, LoopType.Yoyo)
            //.SetEase(Ease.InOutSine);
            Shake();
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if(Input.GetKeyDown(KeyCode.R))
            {
                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.detectCollisions = false;
                rb.useGravity = false;
                //rb.AddForce(Random.insideUnitSphere * Random.Range(7, 10) * 3, ForceMode.VelocityChange);
                //rb.AddTorque(Random.insideUnitSphere * Random.Range(0, 360) * 10);
                //rb.AddTorque(transform.forward * 10f, ForceMode.VelocityChange);
                transform.DORotate(Random.insideUnitSphere * 360, 5);
                GetComponent<Collider>().enabled = false;
            }
#endif
        }

        void Shake()
        {
            float min = .5f;
            float max = 1f;
            transform.DOShakeRotation(1f, Random.Range(min, max), 4, 0f, false)
            .OnComplete(()=>Shake())
            .SetEase(Ease.InOutSine);
        }

        public void Fall()
        {
            
            StartCoroutine(DoFall());
            
            
            IEnumerator DoFall()
            {
                yield return new WaitForSeconds(Random.Range(.2f, .35f));
                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.detectCollisions = false;
                yield return null;
                rb.AddForce(Random.insideUnitSphere * Random.Range(7, 10) * 3, ForceMode.VelocityChange);
                //rb.AddTorque(Random.insideUnitSphere * Random.Range(0, 360) * 10);
                rb.AddTorque(Vector3.up * 360);
                GetComponent<Collider>().enabled = false;   
            }
        }
    }
}