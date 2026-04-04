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
            transform.position = BaloonController.Instance.transform.position;
            
        }

        void SpawnTentacles()
        {
            List<GameObject> tentacles = new List<GameObject>();

            foreach (Transform p in points)
            {
                var g = Instantiate(tentaclePrefab, p);
                g.transform.localPosition = Vector3.zero;
                g.transform.localRotation = Quaternion.identity;
            }
        }
    }
}