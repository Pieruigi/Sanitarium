using Baloon.SaveSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class E5_LargeTentacles : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> tentacles;

        [SerializeField]
        BlooderController blooder;

        string saveId = "e5_large_tentacles";

        public class Data
        {
            /// <summary>
            /// 0: not triggered
            /// 10: visible
            /// 20: completed
            /// </summary>
            public int state;
        }

        int state = 0;

        private void Awake()
        {
            foreach(var tentacle in tentacles) 
                tentacle.SetActive(false);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var rawData = SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData);
                state = data.state;

                if (state == 10)
                {
                    // Show
                    foreach (var tentacle in tentacles)
                        tentacle.SetActive(true);

                    // Play audio
                    PlayAudio();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
            BlooderController.OnSealed += HandleOnBlooderSealed;
            BaloonControlPanel.OnStopped += HandleOnStopped;
        }

        private void OnDisable()
        {
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
            BlooderController.OnSealed -= HandleOnBlooderSealed;
            BaloonControlPanel.OnStopped -= HandleOnStopped;

        }

        private void HandleOnBlooderSealed(BlooderController blooderController)
        {
            if (blooderController != blooder) return;

            if(state == 0) // Not triggered
            {
                state = 10;

                foreach (var t in tentacles)
                    t.SetActive(true);

                PlayAudio();
            }
        }

        private void HandleOnStopped()
        {
            // Check the balloon distance from this object
            var dist = Vector3.Distance(transform.position, BaloonController.Instance.transform.position);

            if(dist < 10) // Let's say 10 meters
            {
                if(state == 10)
                {
                    state = 20; // Completed

                    foreach (var t in tentacles)
                        t.SetActive(false);

                    StopAudio();
                }
                
            }
        }

        

        private void HandleOnUpdateDataEntry()
        {
            Data data = new Data();
            data.state = state;
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        void PlayAudio()
        {

        }

        void StopAudio()
        {

        }
    }
}