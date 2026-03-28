using System;
using UnityEngine;

namespace Baloon
{
    public class BaloonBoilerLeak : MonoBehaviour
    {
        [SerializeField]
        AudioSource explosionAudioSource;

        [SerializeField]
        AudioSource runningAudioSource;

        [SerializeField]
        GameObject bolt;

        [SerializeField]
        ParticleSystem particlePrefab;

        [SerializeField]
        Interactor interactor;

        ParticleSystem particle;

        bool damaged = false;

        int hit = 3;

        bool repairing = false;
        float repairElapsed = 0;
        float repairTime = 1;   

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (repairing && damaged)
            {
                repairElapsed += Time.deltaTime;
                if(repairElapsed > repairTime)
                {
                    repairElapsed -= repairTime;

                    hit--;
                    if (hit < 0) hit = 0;

                    if(hit == 0)
                    {
                        StopLeaking();
                    }
                }
            }
        }

        private void OnEnable()
        {
            Interactor.OnInteractionStarted += HandleOnInteractionStarted;
            Interactor.OnInteractionStopped += HandleOnInteractionStopped;
        }

        private void OnDisable()
        {
            Interactor.OnInteractionStarted -= HandleOnInteractionStarted;
            Interactor.OnInteractionStopped -= HandleOnInteractionStopped;
        }

        private void HandleOnInteractionStarted(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            if (!RepairToolController.Instance.Equipped) return;

            if (!damaged) return;

            repairing = true;
            repairElapsed = 0;
        }

        private void HandleOnInteractionStopped(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            repairing = false;
        }

        public void StartLeaking()
        {
            damaged = true;
            hit = 3;
            bolt.SetActive(false);

            // Instantiate new particle
            particle = Instantiate(particlePrefab, transform);
            particle.transform.localPosition = Vector3.zero;
            particle.transform.localRotation = Quaternion.identity;

            // Audio
            explosionAudioSource.Play();
            runningAudioSource.Play();
        }

        void StopLeaking()
        {
            damaged = false;
            repairing = false;

            bolt.SetActive(true);

            particle.Stop();

            runningAudioSource.Play();

            Destroy(particle.gameObject, 2f);
            
        }

        public void Hit()
        {
            hit--;
            if (hit == 0) StopLeaking();
        }
    }
}