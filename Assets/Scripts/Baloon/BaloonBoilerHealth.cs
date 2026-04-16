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

        [SerializeField]
        [Range(0f,1f)]
        float health;

        //public float Health => health;

        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

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
            if (health <= 0f) return false;
            TakeDamage(.25f);
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

            Repair(.25f);
        }
    }
}