using System;
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

       
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            textField.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

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

            

            if (interactable)
                textField.gameObject.SetActive(true);
            else 
                textField.gameObject.SetActive(false);
        }
    }
}