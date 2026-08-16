using Baloon.SaveSystem;
using System;
using UnityEngine;

namespace Baloon
{
    public class BlooderSealedSaver : MonoBehaviour
    {
        [SerializeField]
        BlooderController blooder;

        bool save = false;

        [SerializeField]
        string saveId = "blooder_saver_";

        class Data
        {
            public bool save;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // Save data
            string rawData = SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData);
                save = data.save;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            BlooderController.OnSealed += HandleOnBlooderSealed;
            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
        }

        private void OnDisable()
        {
            BlooderController.OnSealed -= HandleOnBlooderSealed;
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
        }

        private void HandleOnUpdateDataEntry()
        {
            var data = new Data();
            data.save = save;
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        private void HandleOnBlooderSealed(BlooderController blooderController)
        {
            if (blooder != blooderController) return;

            save = true;    
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (!save) return;

            save = false;

            SaveManager.Instance.Save();
        }
    }

}
