using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Baloon
{
    public class VoiceController : Singleton<VoiceController>
    {
        [System.Serializable]
        class VoiceData
        {
            [SerializeField]
            public AudioClip clip;

            [SerializeField]
            public float volume;

            [SerializeField]
            public List<TextData> texts;
        }

        [System.Serializable]
        class TextData
        {
            [SerializeField]
            public string key;

            [SerializeField]
            public float duration;

            [SerializeField]
            public float delay;
        }

        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        TMP_Text subtitleUI;

        [SerializeField]
        List<VoiceData> data;

        int currentIndex = -1;
        int currentTextIndex = -1;
        float textTime = 0;
        

        protected override void Awake()
        {
            base.Awake();
            subtitleUI.transform.parent.gameObject.SetActive(false);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (currentIndex < 0) return;

            textTime -= Time.deltaTime;
            if(textTime < 0)
            {
                if (!subtitleUI.gameObject.activeSelf) // Delay
                {
                    // Activate subtitle
                    subtitleUI.transform.parent.gameObject.SetActive(true);

                    // Set duration
                    textTime += data[currentIndex].texts[currentTextIndex].duration;
                }
                else // Duration
                {
                    // Activate subtitle
                    subtitleUI.transform.parent.gameObject.SetActive(false);

                    // Set next 
                    currentTextIndex++;

                    if(currentTextIndex >= data[currentIndex].texts.Count)
                    {
                        // Stop
                        currentIndex = -1;
                    }
                    else
                    {
                        textTime += data[currentIndex].texts[currentTextIndex].delay;
                    }
                }
            }
        }

        public void Talk(int index)
        {
            currentIndex = index;

            // Get data
            var d = data[index];

            // Set audio
            audioSource.clip = d.clip;
            audioSource.volume = d.volume;
            audioSource.Play();

            // Hide text
            subtitleUI.transform.parent.gameObject.SetActive(false);

            // Init text index and delay
            currentTextIndex = 0;
            textTime = d.texts[currentTextIndex].delay;
        }
    }

}
