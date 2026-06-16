using Baloon.SaveSystem;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Baloon
{

    public class BlooderController : MonoBehaviour
    {
        public delegate void SealedDelegate(BlooderController blooderController);
        public static SealedDelegate OnSealed;

        public delegate void SealingDelegate(BlooderController blooderController, float progress);
        public static SealingDelegate OnSealing;

        public delegate void StartedDelegate(BlooderController blooderController, bool isSealed);
        public static StartedDelegate OnStarted;

        [SerializeField]
        TurnLever blooderLever;

        [SerializeField]
        Transform blood;

        [SerializeField]
        float bloodCompletedY = 0f;

        [SerializeField]
        AudioSource crankAudioSource;

        [SerializeField]
        AudioSource moaningAudioSource;

        [SerializeField]
        AudioSource hurtAudioSource;

        [SerializeField]
        AudioSource drainAudioSource;

        bool completed = false;
        bool pushed = false;


        public bool Sealed => completed;
        

        [SerializeField]
        string saveId;

        class Data
        {
            public bool completed;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // Load data
            var rawData = SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData); 
                completed = data.completed;
                var pos = blood.localPosition;
                pos.y = bloodCompletedY;
                blood.localPosition = pos;
                blood.gameObject.SetActive(false);
                blooderLever.ForceCompleted();
            }

            OnStarted?.Invoke(this, completed);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if(!pushed || completed) return;

            Debug.Log($"TEST - NormalizedProgress:{blooderLever.NormalizedProgress}");
            if (blooderLever.NormalizedProgress == 1)
            {
                completed = true;
                crankAudioSource.Stop();
                drainAudioSource.Stop();
                moaningAudioSource.Play();
                hurtAudioSource.Play();
                blood.gameObject.SetActive(false);
                CameraShake.Instance.PlayBlooderScream();
                OnSealed?.Invoke(this);

                SaveManager.Instance.Save();
            }
            else
            {
                // Move blood
                var y = Mathf.Lerp(0, bloodCompletedY, blooderLever.NormalizedProgress);
                var pos = blood.localPosition;
                pos.y = y;
                blood.localPosition = pos;

                OnSealing?.Invoke(this, blooderLever.NormalizedProgress);
            }
            
        }

        private void OnEnable()
        {
            blooderLever.OnPushed += HandleOnPushed;
            blooderLever.OnReleased += HandleOnReleased;
            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
        }

        private void OnDisable()
        {
            blooderLever.OnPushed -= HandleOnPushed;
            blooderLever.OnReleased -= HandleOnReleased;
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
        }

        private void HandleOnUpdateDataEntry()
        {
            var data = new Data();
            data.completed = completed;
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        private void HandleOnPushed()
        {
            if (completed) return;
            pushed = true;
            if (!crankAudioSource.isPlaying)
            {
                drainAudioSource.Play();
                crankAudioSource.Play();
            }
            else
            {
                //hurtAudioSource.UnPause();
                crankAudioSource.UnPause(); 
            }
        }

        private void HandleOnReleased()
        {
            pushed = false;
            if (crankAudioSource.isPlaying)
            {
                crankAudioSource.Pause();
                drainAudioSource.Pause();
            }
                
        }
    }
}