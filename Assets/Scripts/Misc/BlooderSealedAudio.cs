using System;
using UnityEngine;

namespace Baloon
{
    public class BlooderSealedAudio : MonoBehaviour
    {
        [SerializeField]
        BlooderController blooder;

        //[SerializeField]
        AudioSource audioSource;

        [SerializeField]
        float delay = 0;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

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
            BlooderController.OnSealed += HandleOnSealed;
        }

        private void OnDisable()
        {
            BlooderController.OnSealed -= HandleOnSealed;
        }

        private void HandleOnSealed(BlooderController blooderController)
        {
            if (blooder != blooderController) return;

            audioSource.PlayDelayed(delay);
        }
    }
}