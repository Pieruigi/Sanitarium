using Baloon.SaveSystem;
using System;
using UnityEngine;

namespace Baloon.UI
{
    public class GameSaver : MonoBehaviour
    {
        [SerializeField]
        BasePlatform platform;

        [SerializeField]
        BlooderController blooder;

        bool blooderSealed = false;

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
            BaloonControlPanel.OnStopped += HandleOnStopped;
            
        }

        private void OnDisable()
        {
            BaloonControlPanel.OnStopped -= HandleOnStopped;
            
        }

        

        private void HandleOnStopped()
        {
            if (blooder.Sealed || BasePlatform.CurrentPlatform != platform) return;
                       

            SaveManager.Instance.Save();
        }
    }
}