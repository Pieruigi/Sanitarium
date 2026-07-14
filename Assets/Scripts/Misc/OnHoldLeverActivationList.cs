using Baloon.SaveSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{

    public class OnHoldLeverActivationList : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> activationList;

        [SerializeField]
        List<GameObject> deactivationList;

        [SerializeField]
        HoldLever lever;

        [SerializeField]
        bool onRelease;

        bool triggered = false;

        class Data
        {
            public bool triggered;
        }

        [SerializeField]
        string saveId;

        private void Awake()
        {
            foreach(var obj in activationList)
                obj.SetActive(false);
            foreach(var obj in deactivationList)
                obj.SetActive(true);
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
                    foreach (var obj in activationList)
                        obj.SetActive(true);
                    foreach (var obj in deactivationList)
                        obj.SetActive(false);
                }

            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {

            lever.OnPushed += HandleLeverOnPushed;
            lever.OnReleased += HandleLeverOnRelease;
            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
        }

        private void OnDisable()
        {
            lever.OnPushed -= HandleLeverOnPushed;
            lever.OnReleased -= HandleLeverOnRelease;
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
        }

        private void HandleOnUpdateDataEntry()
        {
            var data = new Data();
            data.triggered = triggered;
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        private void HandleLeverOnPushed()
        {
            if (onRelease || triggered) return;
            triggered = true;
            foreach (var obj in activationList)
                obj.SetActive(true);
            foreach (var obj in deactivationList)
                obj.SetActive(false);
        }

        private void HandleLeverOnRelease()
        {
            if (!onRelease || triggered) return;
            triggered = true;
            foreach (var obj in activationList)
                obj.SetActive(true);
            foreach (var obj in deactivationList)
                obj.SetActive(false);
        }
    }
}