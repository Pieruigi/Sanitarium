using Baloon.SaveSystem;
using System;
using System.Collections;
using UnityEngine;

namespace Baloon
{
    public class BaloonBoilerHealth : Singleton<BaloonBoilerHealth>
    {
        public delegate void DamageTakenDelegate(float oldHealth, float newHealth);
        public static DamageTakenDelegate OnDamageTaken;

        public delegate void RepairedDelegate(float oldHealth, float newHealth);
        public static RepairedDelegate OnRepaired;

        public static readonly float DamageStep = .4f;

        [SerializeField]
        [Range(0f,1f)]
        float health;

        string saveId = "boiler_health";

        class Data
        {
            public float health;
        }
        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var rawData = SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonUtility.FromJson<Data>(rawData);
                health = data.health;
            }
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    TryTakeSingleDamage();
            //}
            //else if (Input.GetKeyDown(KeyCode.C))
            //{
            //    SingleRepair();
            //}

#endif
        }

        private void OnEnable()
        {
            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
        }

        private void OnDisable()
        {
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
        }

        private void HandleOnUpdateDataEntry()
        {
            var data = new Data();  
            data.health = health;
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        public void TakeDamage(float amount)
        {
            if (health <= 0f) return;

            var oldHealth = health;
            health -= amount;
            if(health < 0f) health = 0f;

            OnDamageTaken?.Invoke(oldHealth, health);
        }

        public bool TryTakeSingleDamage()
        {
            Debug.Log("TEST - Damage");
            if (health <= 0f) return false;
            TakeDamage(DamageStep);
            return true;
        }

        

        public void Repair(float amount)
        {
            var oldHealth = health;
            health += amount;
            if (health > 1f) health = 1f;

            OnRepaired?.Invoke(oldHealth, health);
        }

        public void SingleRepair()
        {
            if (health >= 1) return;

            Repair(DamageStep);
        }
    }
}