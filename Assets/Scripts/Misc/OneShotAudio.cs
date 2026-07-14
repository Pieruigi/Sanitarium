using Baloon.SaveSystem;
using UnityEngine;

namespace Baloon
{
    public class OneShotAudio : MonoBehaviour
    {
        [SerializeField]
        float delay = 0;

        bool triggered = false;

        [SerializeField]
        AudioSource audioSource;

        class Data
        {
            public bool triggered;
        }

        [SerializeField]
        string saveId;

        private void Awake()
        {
            if(!audioSource)
                audioSource = GetComponent<AudioSource>();
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
            if(!other.CompareTag("Player") || triggered) return;

            triggered = true;

            
            audioSource.PlayDelayed(delay);
            
        }
    }
}