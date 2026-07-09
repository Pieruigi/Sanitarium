using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;

namespace Baloon
{
    public class BloodOcean : MonoBehaviour
    {
        [SerializeField]
        float minHeight;

        [SerializeField]
        float maxHeight;


        float step = 0;

        private void Awake()
        {
            var pos = transform.position;
            pos.y = minHeight;
            transform.position = pos;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            BlooderController.OnSealed += HandleOnBlooderSealed;
            BlooderController.OnStarted += HandleOnBlooderStarted;
        }

        private void OnDisable()
        {
            BlooderController.OnSealed -= HandleOnBlooderSealed;
            BlooderController.OnStarted -= HandleOnBlooderStarted;
        }

        private void HandleOnBlooderStarted(BlooderController blooderController, bool isSealed)
        {
            if(step <= 0)
            {
                var count = FindObjectsByType<BlooderController>(FindObjectsSortMode.None).Length;
                step = (maxHeight - minHeight) / count;
            }

            if(isSealed)
            {
                var pos = transform.position;
                pos.y += step;
                transform.position = pos;
            }

        }

        private void HandleOnBlooderSealed(BlooderController blooderController)
        {
            var h = transform.position.y;
            h += step;
            transform.DOMoveY(h, 1f);
        }
    }
}