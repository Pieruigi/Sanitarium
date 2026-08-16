using Baloon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Baloon.UI
{
    public class StationShutDownUI : MonoBehaviour
    {
        [SerializeField]
        GameObject text;

        [SerializeField]
        LocalizeStringEvent localizeStringEvent;

        private void Awake()
        {

        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            text.SetActive(false);
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
            StartCoroutine(ShowText());

            IEnumerator ShowText()
            {
                var sr = localizeStringEvent.StringReference;
                if (sr.TryGetValue("left", out IVariable v))
                {
                    var l = FindObjectsByType<BlooderController>(FindObjectsSortMode.None).Count(b => !b.Sealed) - 1; // In the demo we only play 2 blooders, so we exclude the others
                    (v as IntVariable).Value = l;
                }

                yield return new WaitForSeconds(2f);
                text.SetActive(true);
                yield return new WaitForSeconds(3f);
                text.SetActive(false);
            }



        }
    }
}