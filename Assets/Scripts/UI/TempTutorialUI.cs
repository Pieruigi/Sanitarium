using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Baloon.UI
{

    public class TempTutorialUI : MonoBehaviour
    {
        [SerializeField]
        Interactor interactor;

        [SerializeField]
        TMP_Text textField;

        bool show = false;

       
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            show = false;
            textField.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (StationReachedUI.Instance.IsVisible && textField.gameObject.activeSelf)
            {
                show = false;
                textField.gameObject.SetActive(false);
            }
                

        }

        private void OnEnable()
        {
            Interactor.OnHint += HandleOnHint;
        }

        private void OnDisable()
        {
            Interactor.OnHint -= HandleOnHint;
        }

        private void HandleOnHint(Interactor interactor, bool interactable)
        {
            
            if (interactor != this.interactor) return;

            if (StationReachedUI.Instance.IsVisible)
            {
                show = false;
                textField.gameObject.SetActive(false);
                return;
            }
            

            show = interactable;

            if (interactable)
                StartCoroutine(ShowDelayed());
            else 
                textField.gameObject.SetActive(false);

            IEnumerator ShowDelayed()
            {
                yield return new WaitForSeconds(.25f);

                if(show)
                    textField.gameObject.SetActive(true);
            }
        }
    }
}