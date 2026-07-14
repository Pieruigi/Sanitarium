using Baloon.SaveSystem;
using DG.Tweening;
using UnityEngine;

namespace Baloon
{
    public class DoorTrigger : MonoBehaviour
    {
        [SerializeField]
        Transform doorPivot;

        [SerializeField]
        float angle = 90;

        [SerializeField]
        float time = 1f;

        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        float audioDelay = 0;

        bool triggered = false;

        [SerializeField]
        string saveId;

        class Data
        {
            public bool triggered;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var rawData = SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData);
                triggered = data.triggered;
                if (triggered)
                {
                    doorPivot.localEulerAngles = Vector3.up * angle;
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
            var data = new Data();
            data.triggered = triggered;
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || triggered) return;

            triggered = true;

            doorPivot.DOLocalRotate(Vector3.up * angle, time).SetEase(Ease.OutQuad);

            if (audioSource)
                audioSource.PlayDelayed(audioDelay);
        }
    }
}