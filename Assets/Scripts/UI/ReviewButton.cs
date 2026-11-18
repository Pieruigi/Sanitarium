using System;
using UnityEngine;
using UnityEngine.UI;

namespace Baloon.UI
{
    public class ReviewButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnClick()
        {
            Application.OpenURL("steam://openurl/https://store.steampowered.com/app/4839050/Altitude_Zero/#app_reviews_hash");

            GameManager.Instance.ReportGameCompleted();

        }
    }
}