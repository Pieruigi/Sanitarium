using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Baloon.UI
{
    public class DirectionHintUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text hintField;

        [SerializeField]
        GameObject hitMeText;

        private void Awake()
        {
            hintField.enabled = false;
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
            Interactor.OnHint += HandleOnHint;

        }

        private void OnDisable()
        {
            Interactor.OnHint -= HandleOnHint;
        }

        private void HandleOnHint(Interactor interactor, bool interactable)
        {
            var bl = interactor.GetComponentInParent<BaloonLauncherPanel>();
            if (bl == null) return;

            if(interactable == false)// || hitMeText.activeSelf)
            {
                hintField.enabled = false;
            }
            else
            {
                // Set text
                string name = bl.gameObject.name;
                if (name.EndsWith("-N"))
                {
                    hintField.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "direction_n";
                }
                else if (name.EndsWith("-E"))
                {
                    hintField.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "direction_e";
                    //hintField.text = "Fly east";
                }
                else if (name.EndsWith("-S"))
                {
                    hintField.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "direction_s";
                    //hintField.text = "Fly south";
                }
                else if (name.EndsWith("-W"))
                {
                    hintField.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "direction_w";
                    //hintField.text = "Fly west";
                }

                hintField.enabled = true;

            }
        }
    }
}