using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Baloon.UI
{
    public class LoadingText : MonoBehaviour
    {
        [SerializeField]
        GameObject panel;

       

        private void Awake()
        {
            panel.SetActive(false);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void Show()
        {
            panel.SetActive(true);
        }
        
    }
}