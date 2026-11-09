using Baloon.SaveSystem;
using System.Collections;
using UnityEngine;

namespace Baloon
{
    public class OneShotJumpscare : MonoBehaviour
    {
        [SerializeField]
        float delay = 0;

        [SerializeField]
        float duration = 2;

        bool triggered = false;

                
        class Data
        {
            public bool triggered;
        }

        [SerializeField]
        string saveId;

        private void Awake()
        {
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (!string.IsNullOrEmpty(saveId))
            {
                var rawData = SaveManager.Instance.GetRawJsonData(saveId);
                if (!string.IsNullOrEmpty(rawData))
                {
                    var data = JsonUtility.FromJson<Data>(rawData);
                    triggered = data.triggered;

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
        }

        private void OnDisable()
        {
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
        }

        private void HandleOnUpdateDataEntry()
        {
            if (string.IsNullOrEmpty(saveId)) return;
            var data = new Data();
            data.triggered = triggered;
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || triggered) return;

            triggered = true;

            StartCoroutine(Play());

            IEnumerator Play()
            {
                yield return new WaitForSeconds(delay);

                CameraShake.Instance.PlayJumpscare(duration);
            }            

        }
    }
}