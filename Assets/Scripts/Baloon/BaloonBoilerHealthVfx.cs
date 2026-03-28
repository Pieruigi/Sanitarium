using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class BaloonBoilerHealthVfx : MonoBehaviour
    {
        [SerializeField]
        List<Transform> leaks;

        [SerializeField]
        ParticleSystem leakParticlePrefab;

        
        
        List<Transform> usedLeaks = new List<Transform>();

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

                // Instantiate new particle
                var p = Instantiate(leakParticlePrefab, leak);
                p.transform.localPosition = Vector3.zero;
                p.transform.localRotation = Quaternion.identity;

                // Explosion
                leak.Find("1").GetComponent<AudioSource>().Play();
                leak.Find("2").GetComponent<AudioSource>().Play();

                diff -= step;

            }

        }

        
    }
}