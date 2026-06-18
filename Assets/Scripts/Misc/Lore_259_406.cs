using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class Lore_259_406 : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> collection;

        [SerializeField]
        Transform lookTarget;

        bool looking, lookingLast;

        int currentIndex;

        private void Awake()
        {
            
            
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ShowRandom();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if (BasePlatform.CurrentPlatform) return;

            var pos = transform.position;
            pos.y = BaloonController.Instance.transform.position.y;
            transform.position = pos;

            // Check if the player is looking at the lore
            lookingLast = looking;

            var look = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
            var dir = Vector3.ProjectOnPlane(lookTarget.transform.position - Camera.main.transform.position, Vector3.up);
            looking = Vector3.Dot(dir.normalized, look.normalized) > 0.5f;
            
            if(lookingLast != looking && !looking)
            {
                // Switch collection
                ShowRandom();
            }
        }

        void ShowRandom()
        {
            currentIndex = Random.Range(0, collection.Count);
            foreach (var c in collection)
                c.SetActive(false);

            collection[currentIndex].SetActive(true);
        }
    }
}