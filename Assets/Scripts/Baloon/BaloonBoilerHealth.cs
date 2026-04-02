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
            //    TakeDamage(.2f);
            //}
            //else if (Input.GetKeyDown(KeyCode.C))
            //{
            //    Repair(.2f);
            //}

#endif
        }

        public void TakeDamage(float amount)
        {
            var oldHealth = health;
            health -= amount;
            if(health < 0f) health = 0f;

            OnDamageTaken?.Invoke(oldHealth, health);
        }

        public void TakeSingleDamage()
        {
            TakeDamage(.2f);
        }

        public void TakeDoubleDamage()
        {
            TakeDamage(.4f);
        }

        public void Repair(float amount)
        {
            var oldHealth = health;
            health += amount;
            if (health > 1f) health = 1f;

            OnRepaired?.Invoke(oldHealth, health);
        }
    }
}