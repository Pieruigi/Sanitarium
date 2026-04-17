using System;
using UnityEngine;

namespace Baloon
{
    public class CarrionFlyPicker : MonoBehaviour
    {
        [SerializeField]
        CarrionFlyController controller;

        [SerializeField]
        Interactor interactor;

        bool holding = false;

        Rigidbody rb;
        Collider coll;

        private void Awake()
        {
            rb = controller.GetComponent<Rigidbody>();
            coll = controller.GetComponent<Collider>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if (!holding) return;

            // Set position
            var pos = Camera.main.transform.position + Camera.main.transform.forward;
            controller.transform.position = Vector3.Lerp(controller.transform.position, pos, 10f * Time.deltaTime);

            return;
            var targetFwd = Camera.main.transform.up;
            var targetUp = Camera.main.transform.forward;

            var targetRot = Quaternion.LookRotation(targetFwd, targetUp);

            controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, targetRot, 10f * Time.deltaTime);
        }

        private void OnEnable()
        {
            Interactor.OnInteractionStarted += HandleOnInteractionStarted;
            Interactor.OnInteractionStopped += HandleOnInteractionStopped;
        }

        private void OnDisable()
        {
            Interactor.OnInteractionStarted -= HandleOnInteractionStarted;
            Interactor.OnInteractionStopped -= HandleOnInteractionStopped;
        }

        private void HandleOnInteractionStarted(Interactor interactor)
        {
            if (this.interactor != interactor || !controller.IsDead || !LeftHand.Instance.IsFree || holding) return;
          
            LeftHand.Instance.IsFree = false;
            holding = true;

            rb.isKinematic = true;
            coll.enabled = false;
        }

        private void HandleOnInteractionStopped(Interactor interactor)
        {
            if (this.interactor != interactor || !controller.IsDead || LeftHand.Instance.IsFree || !holding) return;

            holding = false;
            LeftHand.Instance.IsFree = true;
            rb.isKinematic = false;
            coll.enabled = true;
        }
    }
}