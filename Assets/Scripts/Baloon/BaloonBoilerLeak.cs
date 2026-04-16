using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
        AudioSource hitAudioSource;

        [SerializeField]
        List<AudioClip> hitAudioClips;

        [SerializeField]
        GameObject bolt;

        [SerializeField]
        ParticleSystem particlePrefab;

        [SerializeField]
        ParticleSystem sparksPrefab;

        


        [SerializeField]
        Interactor interactor;

        

        ParticleSystem particle;

        bool damaged = false;
        public bool Damaged => damaged;

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
            RepairToolEventListener.OnHit += HandleOnAnimationHit;
            BaloonControlPanel.OnStarted += HandleOnBaloonStarted;
            BaloonControlPanel.OnStopped += HandleOnBaloonStopped;
        }

        private void OnDisable()
        {
            Interactor.OnInteractionStarted -= HandleOnInteractionStarted;
            Interactor.OnInteractionStopped -= HandleOnInteractionStopped;
            RepairToolEventListener.OnHit -= HandleOnAnimationHit;
            BaloonControlPanel.OnStarted -= HandleOnBaloonStarted;
            BaloonControlPanel.OnStopped += HandleOnBaloonStopped;
        }

        private void HandleOnBaloonStarted()
        {
            //throw new System.NotImplementedException();

            if (damaged)
            {
                if (particle) particle.Play();
                runningAudioSource.Play();
            }
        }

        private void HandleOnBaloonStopped()
        {
            if (damaged)
            {
                if(particle) particle.Stop();
                runningAudioSource.Stop();
            }
        }

        private void HandleOnAnimationHit()
        {
            if (!repairing) return;

            var sparkles = Instantiate(sparksPrefab, transform);
            sparkles.transform.localPosition = Vector3.zero;
            sparkles.transform.localRotation = Quaternion.identity;

            PlayHitAudio();

            // Shake camera
            CameraShake.Instance.PlayWrenchHit();
            
            Destroy(sparkles.gameObject, 3f);
        }

        private void HandleOnInteractionStarted(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            if (!RepairToolController.Instance.Equipped) return;

            if (!damaged) return;

            RepairToolController.Instance.StartRepairAnimation();

            repairing = true;
            repairElapsed = 0;
        }

        private void HandleOnInteractionStopped(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            repairing = false;

            RepairToolController.Instance.StopRepairAnimation();
        }

        public void StartLeaking()
        {
            damaged = true;
            hit = 3;

            bolt.transform.DOLocalMoveZ(5f, 1f).SetDelay(.2f).OnComplete(() => { bolt.transform.localPosition = Vector3.zero; bolt.transform.localRotation = Quaternion.identity; bolt.SetActive(false); });
            

            // Instantiate new particle
            particle = Instantiate(particlePrefab, transform);
            particle.transform.localPosition = Vector3.zero;
            particle.transform.localRotation = Quaternion.identity;

            // Audio
            explosionAudioSource.Play();
            runningAudioSource.Play();

            // Shake camera
            CameraShake.Instance.PlayWrenchHit();

        }

        void StopLeaking()
        {
            

            StartCoroutine(DoRepair());

            IEnumerator DoRepair()
            {
                yield return new WaitForSeconds(.25f);

                damaged = false;
                repairing = false;

                bolt.SetActive(true);

                particle.Stop();

                runningAudioSource.Stop();

                Destroy(particle.gameObject, 2f);

                //BaloonBoilerHealth.Instance.Repair(.2f);
                BaloonBoilerHealth.Instance.SingleRepair();
            }

            
            
        }

        void PlayHitAudio()
        {
            hitAudioSource.clip = hitAudioClips[Random.Range(0, hitAudioClips.Count)];
            hitAudioSource.Play();
        }

        public void Hit()
        {
            hit--;
            if (hit == 0) StopLeaking();
        }
    }
}