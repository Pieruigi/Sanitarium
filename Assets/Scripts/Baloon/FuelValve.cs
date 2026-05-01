using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace Baloon
{
    public class FuelValve : MonoBehaviour
    {
        [SerializeField]
        HoldLever lever;

        [SerializeField]
        FuelTower tower;

        [SerializeField]
        Transform blood;

        [SerializeField]
        AudioSource clinkAudioSource;

        float gasLevel = 0;

        bool open = false;

        
        float refuelRate = .1f;

        float bloodEmpty = -.205f;
        float bloodFull = 0f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!open) return;
            
            var boiler = BoilerController.Instance;
            if (!boiler.IsFull())
            {
                if (boiler.TryRefuel(refuelRate * Time.deltaTime))
                    UpdateGasLevel();
            }
        }

        private void OnEnable()
        {
            lever.OnPushed += HandleOnPushed;
            lever.OnReleased += HandleOnRelease;
            tower.OnEnter += HandleOnEnter;
        }

        private void OnDisable()
        {
            lever.OnPushed -= HandleOnPushed;
            lever.OnReleased -= HandleOnRelease;
            tower.OnEnter -= HandleOnEnter;
        }

        void UpdateGasLevel()
        {
            gasLevel = 1f - BoilerController.Instance.GasLeft;

            var l = Mathf.Lerp(bloodEmpty, bloodFull, gasLevel);
            var pos = blood.transform.localPosition;
            pos.z = l;
            blood.transform.localPosition = pos;
        }

        private void HandleOnEnter()
        {
            UpdateGasLevel();
        }

        private void HandleOnPushed()
        {
            open = true;
            clinkAudioSource.Play();
        }

        private void HandleOnRelease()
        {
            open = false;
            clinkAudioSource.Play();
        }
    }
}