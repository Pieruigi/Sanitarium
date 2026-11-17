using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Baloon
{

    public class PathTile : MonoBehaviour
    {
        [SerializeField]
        BlooderController blooder;

        [SerializeField]
        bool walkable = false;

        public bool Walkable => walkable;

        public bool IsShaking { get; set; } = true;

        
        private void Awake()
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //if(!blooder.Sealed)
            //    Shake();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            BlooderController.OnStarted += HandleOnBlooderStarted;
        }

        private void OnDisable()
        {
            BlooderController.OnStarted -= HandleOnBlooderStarted;
        }

        private void HandleOnBlooderStarted(BlooderController blooderController, bool isSealed)
        {
            if (blooderController != blooder) return;

            if (!isSealed || !walkable)
            {
                IsShaking = true;
                Shake();
            }
            else
            {
                IsShaking = false;
            }
                
        }

        void Shake()
        {
            float min = .5f;
            float max = 1f;
            transform.DOShakeRotation(1f, Random.Range(min, max) * 1.5f, 4, 0f, false)
            .OnComplete(()=>Shake())
            .SetEase(Ease.InOutSine);
        }

        public void Fall()
        {
            
            StartCoroutine(DoFall());
            
            
            IEnumerator DoFall()
            {
                yield return new WaitForSeconds(Random.Range(.2f, .35f));
                DOTween.KillAll();
                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.detectCollisions = false;
                yield return null;
                rb.AddForce(Random.insideUnitSphere * Random.Range(7, 10) * 3, ForceMode.VelocityChange);
                rb.AddTorque(Random.insideUnitSphere * Random.Range(0, 360) * 2);
                GetComponent<Collider>().enabled = false;   
            }
        }

        public void StopShaking()
        {
            if (!IsShaking) return;
            IsShaking = false;
            transform.DOKill();
            // Fast shake and stop
            transform.DOShakeRotation(.5f, strength: 5f, vibrato:8, fadeOut:false)
            .OnComplete(() => transform.localRotation = Quaternion.identity);
            
            var audioSource = GetComponent<AudioSource>();
            audioSource.Play();
        }
    }
}