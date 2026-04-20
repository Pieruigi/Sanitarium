using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Baloon
{
    public class BaloonBoilerHealthVfx : MonoBehaviour
    {
        [SerializeField]
        List<BaloonBoilerLeak> leaks;

        public IList<BaloonBoilerLeak> Leaks
        {
            get { return leaks.AsReadOnly(); }
        }

        [SerializeField]
        ParticleSystem leakParticlePrefab;



        public BaloonBoilerLeak NextToHit { get; set; } = null;
        


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
            
            while(diff > Mathf.Epsilon)
            {
                
                BaloonBoilerLeak leak = null;
                if (NextToHit)
                {
                    leak = NextToHit;
                    NextToHit = null;
                }
                else
                {
                    var list = leaks.Where(l => l.Damaged == false).ToList();
                    int index = Random.Range(0, list.Count);
                    leak = list[index];
                }
                
                leak.StartLeaking();

                diff -= BaloonBoilerHealth.DamageStep;

            }

        }

        
    }
}