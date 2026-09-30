using Baloon.SaveSystem;
using System;
using System.Collections;
using UnityEngine;

namespace Baloon
{
    
    public class ExplosiveSwitch : MonoBehaviour
    {
        [SerializeField]
        GameObject rockGroup;

        [SerializeField]
        HoldButton button;

        [SerializeField]
        Collider _collider;

        [SerializeField]
        AudioSource switchAudioSource;

        [SerializeField]
        AudioSource explosionAudioSource;

        bool pushed = false;

        [SerializeField]
        string saveId;

        class Data
        {
            public bool pushed;
        }

        private void Awake()
        {
            
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var rawData = SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData);
                pushed = data.pushed;

                if (pushed)
                {
                    _collider.enabled = false;
                    rockGroup.SetActive(false);
                    button.ForcePushed();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnEnable()
        {
            button.OnPushed += HandleOnPushed;
            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
        }

        private void OnDisable()
        {
            button.OnPushed -= HandleOnPushed;
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
        }

        private void HandleOnUpdateDataEntry()
        {
            var data = new Data();
            data.pushed = pushed;//  Mathf.Max(gasLeft, 0.2f);
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        private void HandleOnPushed()
        {
            if (pushed) return;
            pushed = true;
            _collider.enabled = false;

            rockGroup.SetActive(false);

            StartCoroutine(DoExplosion());

            IEnumerator DoExplosion()
            {
                switchAudioSource.Play();

                yield return new WaitForSeconds(.5f);
                CameraShake.Instance.PlayBlooderScream();
                explosionAudioSource.Play();
            }
        }
    }
}