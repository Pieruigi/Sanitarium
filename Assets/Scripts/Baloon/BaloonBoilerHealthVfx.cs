using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Baloon
{
    public class BaloonBoilerHealthVfx : MonoBehaviour
    {
        [SerializeField]
        List<BaloonBoilerLeak> leaks;

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
                var list = leaks.Where(l => l.Damaged == false).ToList();
                int index = Random.Range(0, list.Count);
                var leak = list[index];
                
                leak.StartLeaking();

                diff -= step;

            }

        }

        
    }
}