using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class BaloonBoilerHealthVfx : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> leaks;

        [SerializeField]
        ParticleSystem leakParticlePrefab;


        float step = .2f;



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
            BaloonBoilerHealth.OnDamageTaken += HandleOnDamageTaken;
        }

        private void OnDisable()
        {
            BaloonBoilerHealth.OnDamageTaken -= HandleOnDamageTaken;
        }

        private void HandleOnDamageTaken(float oldHealth, float newHealth)
        {
            var diff = oldHealth - newHealth;   
            while(diff >= 0)
            {
                int index = Random.Range(0, leaks.Count);
                var leak = leaks[index];
                leaks.Remove(leak);

                leak.GetComponent<BaloonBoilerLeak>().StartLeaking();

                diff -= step;

            }

        }

        
    }
}