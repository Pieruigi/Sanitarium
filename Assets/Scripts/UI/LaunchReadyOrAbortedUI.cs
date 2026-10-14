using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Baloon.UI
{
    public class LaunchReadyOrAbortedUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text textField;

        [SerializeField]
        GameObject textRoot;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            textRoot.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            BaloonLauncherButtonFx.OnReadyToLaunch += HandleOnReadyToLaunch;
            BaloonLauncherButtonFx.OnLaunchAborted += HandleOnLaunchAborted;
        }

        private void OnDisable()
        {
            BaloonLauncherButtonFx.OnReadyToLaunch -= HandleOnReadyToLaunch;
            BaloonLauncherButtonFx.OnLaunchAborted -= HandleOnLaunchAborted;
        }

        private void HandleOnReadyToLaunch()
        {
            Show(true);
        }

        private void HandleOnLaunchAborted()
        {
            Show(false);
        }

        void Show(bool ready)
        {

            StartCoroutine(DoShow(ready));

            IEnumerator DoShow(bool ready)
            {
                // Set the right key
                string key = ready ? "launch_ready" : "launch_aborted";
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