using StarterAssets;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;

namespace Baloon.UI
{
    public class AltitudeAlertUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text textField;

        [SerializeField]
        GameObject textRoot;

        FirstPersonController player;
        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            textRoot.SetActive(false);

            player = FindFirstObjectByType<FirstPersonController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            BaloonSpeakerController.OnAltitudeAlert += HandleOnAltitudeAlert;
            BaloonPathManager.OnPathCleared += HandleOnPathCleared;
        }

        private void OnDisable()
        {
            BaloonSpeakerController.OnAltitudeAlert -= HandleOnAltitudeAlert;
            BaloonPathManager.OnPathCleared -= HandleOnPathCleared;
        }

        private void HandleOnPathCleared()
        {
            textRoot.SetActive(false);
        }

        private void HandleOnAltitudeAlert(bool tooLow)
        {
            ShowAlert(tooLow);
        }

        void ShowAlert(bool tooLow)
        {

            StartCoroutine(DoShow(tooLow));

            IEnumerator DoShow(bool tooLow)
            {
                if (player.Doomed)
                {
                    textRoot.SetActive(false);
                    yield break;
                }

                // Set the right key
                string key = tooLow ? "alert_gain_altitude" : "alert_lose_altitude";
                textField.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = key;

                yield return new WaitForEndOfFrame();

                // Show subtitle
                textRoot.SetActive(true);

                // Wait
                yield return new WaitForSeconds(2f);

                // Hide subtitle
                textRoot.SetActive(false);
            }

            
        }
    }
}