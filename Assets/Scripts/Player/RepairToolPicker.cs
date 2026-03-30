using System;
using UnityEngine;

namespace Baloon
{
    public class RepairToolPicker : MonoBehaviour
    {
        [SerializeField]
        Interactor interactor;

        [SerializeField]
        GameObject wrench;

        bool picked = false;

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
            Interactor.OnInteractionStarted += HandleOnInteractionStarted;
        }

        private void OnDisable()
        {
            Interactor.OnInteractionStarted -= HandleOnInteractionStarted;
        }

        private void HandleOnInteractionStarted(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            if (!picked)
            {
                picked = true;
                wrench.SetActive(false);
                RepairToolController.Instance.ReportPickedUp();
            }
            else
            {
                //picked = false;
                //wrench.SetActive(true);
                //RepairToolController.Instance.ReportPutBack();
                PutBack();
            }

        }

        public void PutBack()
        {
            if (!picked) return;
            picked = false;
            wrench.SetActive(true);
            RepairToolController.Instance.ReportPutBack();
        }
    }
}