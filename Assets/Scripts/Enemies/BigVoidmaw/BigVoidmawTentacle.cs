using Baloon.UI;
using System;
using TMPro;
using UnityEngine;

namespace Baloon
{


    public class BigVoidmawTentacle : MonoBehaviour
    {
        Animator animator;

        bool hit = false;

        float hitTime = 5f;
        float hitElapsed = 0f;

        bool isTarget = false;

        public bool IsAttached => !hit;

        BigVoidmawController bigController;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            animator.Play("Attack", 0, 0.7f);
            animator.SetBool("Attached", true);
            bigController = GetComponentInParent<BigVoidmawController>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (hit)
            {
                hitElapsed += Time.deltaTime;
                if (hitElapsed > hitTime)
                {
                    hit = false;
                    animator.SetBool("Attached", true);
                }
            }
        }

        private void OnEnable()
        {
            Interactor.OnInteractionStarted += HandleOnInteractionStarted;
            Interactor.OnInteractionStopped += HandleOnInteractionStopped;
            Interactor.OnHint += HandleOnInteractionHint;
            RepairToolEventListener.OnHit += HandleOnHit;

        }

        private void OnDisable()
        {
            Interactor.OnInteractionStarted -= HandleOnInteractionStarted;
            Interactor.OnInteractionStopped -= HandleOnInteractionStopped;
            Interactor.OnHint -= HandleOnInteractionHint;
            RepairToolEventListener.OnHit -= HandleOnHit;
        }

        private void HandleOnHit()
        {
            if (!isTarget || hit) return;

            hit = true;
            isTarget = false;
            hitElapsed = 0;

            CameraShake.Instance.PlayWrenchHit();

            animator.SetBool("Attached", false);
            RepairToolController.Instance.StopRepairAnimation();

            bigController.ReportTentacleHit();

        }

        private void HandleOnInteractionStopped(Interactor interactor)
        {
            if (interactor != GetComponentInChildren<Interactor>()) return;
            isTarget = false;
            RepairToolController.Instance.StopRepairAnimation();
            //FindFirstObjectByType<DotUI>().HideHold();


        }

        private void HandleOnInteractionStarted(Interactor interactor)
        {
            if (interactor != GetComponentInChildren<Interactor>()) return;
            if (!RepairToolController.Instance.Equipped) return;

            if (hit) return;

            isTarget = true;

            RepairToolController.Instance.StartRepairAnimation();

            FindFirstObjectByType<DotUI>().HideHold();
            

        }

        private void HandleOnInteractionHint(Interactor interactor, bool interactable)
        {
            var localInteractor = GetComponentInChildren<Interactor>();
            if (interactor != localInteractor) return;
            if (hit) return;
            if (!RepairToolController.Instance.Equipped) return;
            

            FindFirstObjectByType<DotUI>().ShowHold();
        }

        public void ForceDetach()
        {
            hit = true;
            hitElapsed = 0;
            animator.SetBool("Attached", false);
        }
    }
}