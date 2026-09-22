using Baloon.SaveSystem;
using Baloon.UI;
using System;
using UnityEngine;

namespace Baloon
{

    public class FuelSaver : MonoBehaviour
    {
        [SerializeField]
        HoldLever lever;

        bool save = false;

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
            lever.OnReleased += HandleOnLeverReleased;
        }

        private void OnDisable()
        {
            lever.OnReleased -= HandleOnLeverReleased;
        }

        private void HandleOnLeverReleased()
        {
            save = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!save || !other.CompareTag("Player")) return;            

            save = false;    
            
            SaveManager.Instance.Save();

        }

        
    }
}