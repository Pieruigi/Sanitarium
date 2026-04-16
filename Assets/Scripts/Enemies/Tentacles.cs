using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class Tentacles : MonoBehaviour
    {
        [SerializeField]
        GameObject tentaclePrefab;

        [SerializeField]
        List<Transform> points;

        bool keepPosition = false;

        private void Awake()
        {
            transform.position = BaloonController.Instance.transform.position - Vector3.up * 50f;    
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SpawnTentacles();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if(keepPosition)
                transform.position = Vector3.Lerp(transform.position, BaloonController.Instance.transform.position, 10 * Time.deltaTime);
            
        }

        void SpawnTentacles()
        {
            keepPosition = true;

            List<GameObject> tentacles = new List<GameObject>();

            foreach (Transform p in points)
            {
                var g = Instantiate(tentaclePrefab, p);
                g.transform.localPosition = Vector3.zero;
                g.transform.localRotation = Quaternion.identity;
            }

            transform.DOMoveY(-50f, 2f).SetDelay(4.5f).SetEase(Ease.InQuad).OnStart(() => { keepPosition = false; });
        }
    }
}