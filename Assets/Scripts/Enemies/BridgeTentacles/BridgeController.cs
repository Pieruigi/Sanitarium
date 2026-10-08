using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{

    public class BridgeController : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> tentacles;

        private void Awake()
        {
            foreach (var tentacle in tentacles)
                tentacle.SetActive(false);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            StartCoroutine(DoPlay());

            IEnumerator DoPlay()
            {
                tentacles[0].SetActive(true);

                // Rotate pivot
                var pivot = tentacles[0].transform.parent;
                pivot.DORotate(Vector3.zero, 1f);

                yield return new WaitForSeconds(2);
            }
        }
    }
}