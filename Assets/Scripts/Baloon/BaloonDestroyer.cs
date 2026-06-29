using StarterAssets;
using System.Collections;
using UnityEngine;

namespace Baloon
{
    public class BaloonDestroyer : MonoBehaviour
    {
        [SerializeField]
        Rigidbody rbBasket, rbBoiler, rbBalloon;

        [SerializeField]
        ParticleSystem explosionParticlePrefab;

        [SerializeField]
        Transform explosionSpawnPoint;

        [SerializeField]
        GameObject controlPanel;


        bool destroyed = false;

        Vector3 balloonForceDir;
        Vector3 balloonTorqueDir;

        private void Awake()
        {
            rbBasket.isKinematic = true;
            rbBoiler.isKinematic = true;
            rbBalloon.isKinematic = true;

            rbBasket.useGravity = false;
            rbBoiler.useGravity = false;
            rbBalloon.useGravity = false;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            if (!destroyed) return;

            rbBalloon.AddForce(balloonForceDir * 30, ForceMode.Acceleration);
            rbBalloon.AddTorque(new Vector3(balloonTorqueDir.x*.01f, balloonTorqueDir.y*.05f, balloonTorqueDir.z*.01f), ForceMode.VelocityChange);
        }

        private void OnEnable()
        {
            FirstPersonController.OnDead += HandleOnDead;
            BaloonBoilerHealth.OnDamageTaken += HandleOnDamageTaken;
        }
        private void OnDisable()
        {
            FirstPersonController.OnDead -= HandleOnDead;
            BaloonBoilerHealth.OnDamageTaken -= HandleOnDamageTaken;
        }

        private void HandleOnDamageTaken(float oldHealth, float newHealth)
        {
            if (newHealth > 0) return;

            // Explode
            StartCoroutine(DoPlayExplosion());

            //IEnumerator DoPlayExplosion()
            //{
            //    FirstPersonController player = FindFirstObjectByType<FirstPersonController>();
            //    player.Doomed = true;
            //    player.Die(PlayerDeadType.BoilerExplosion);

            //    // Play
            //    var explosionParticle = Instantiate(explosionParticlePrefab);
            //    explosionParticle.transform.position = explosionSpawnPoint.position;
            //    explosionParticle.transform.rotation = explosionSpawnPoint.rotation;

            //    //Destroy(explosionParticle.gameObject, 3f);

            //    CameraShake.Instance.PlayJumpscare(1f);

            //    yield return new WaitForSeconds(.25f);

            //    // Launch player
            //    var dir = Vector3.ProjectOnPlane(player.transform.position - explosionParticle.transform.position, Vector3.up);
            //    dir = dir.normalized * 4 + Vector3.up;
            //    var rb = player.GetComponent<Rigidbody>();
            //    rb.AddForce(dir * Random.Range(2f,4f), ForceMode.VelocityChange);
            //    rb.AddTorque(Random.onUnitSphere * Random.Range(1f, 6f), ForceMode.VelocityChange);

            //    // Launch boiler
            //    rbBoiler.transform.parent = null;
            //    rbBoiler.useGravity = true;
            //    rbBoiler.isKinematic = false;
            //    rbBoiler.AddForce(Random.onUnitSphere * Random.Range(2f, 4f), ForceMode.VelocityChange);
            //    rbBoiler.AddTorque(Random.onUnitSphere * Random.Range(1f, 6f), ForceMode.VelocityChange);

            //    // Launch basket
            //    controlPanel.transform.parent = rbBasket.transform;
            //    rbBasket.transform.parent = null;   
            //    rbBasket.useGravity = true;
            //    rbBasket.isKinematic = false;
            //    rbBasket.AddForce(Random.onUnitSphere * Random.Range(1f, 2f), ForceMode.VelocityChange);
            //    rbBasket.AddTorque(Random.onUnitSphere * Random.Range(1f, 6f), ForceMode.VelocityChange);

                

            //    yield break;
            //}
        }

        public IEnumerator DoPlayExplosion(float delay = 0)
        {
            FirstPersonController player = FindFirstObjectByType<FirstPersonController>();
            player.Doomed = true;

            if(delay > 0) yield return new WaitForSeconds(delay);

            player.Die(PlayerDeadType.BoilerExplosion);

            // Play
            var explosionParticle = Instantiate(explosionParticlePrefab);
            explosionParticle.transform.position = explosionSpawnPoint.position;
            explosionParticle.transform.rotation = explosionSpawnPoint.rotation;

            //Destroy(explosionParticle.gameObject, 3f);

            CameraShake.Instance.PlayJumpscare(1f);

            yield return new WaitForSeconds(.25f);

            // Launch player
            var dir = Vector3.ProjectOnPlane(player.transform.position - explosionParticle.transform.position, Vector3.up);
            dir = dir.normalized * 4 + Vector3.up;
            var rb = player.GetComponent<Rigidbody>();
            rb.AddForce(dir * Random.Range(2f, 4f), ForceMode.VelocityChange);
            rb.AddTorque(Random.onUnitSphere * Random.Range(1f, 6f), ForceMode.VelocityChange);

            // Launch boiler
            rbBoiler.transform.parent = null;
            rbBoiler.useGravity = true;
            rbBoiler.isKinematic = false;
            rbBoiler.AddForce(Random.onUnitSphere * Random.Range(2f, 4f), ForceMode.VelocityChange);
            rbBoiler.AddTorque(Random.onUnitSphere * Random.Range(1f, 6f), ForceMode.VelocityChange);

            // Launch basket
            controlPanel.transform.parent = rbBasket.transform;
            rbBasket.transform.parent = null;
            rbBasket.useGravity = true;
            rbBasket.isKinematic = false;
            rbBasket.AddForce(Random.onUnitSphere * Random.Range(1f, 2f), ForceMode.VelocityChange);
            rbBasket.AddTorque(Random.onUnitSphere * Random.Range(1f, 6f), ForceMode.VelocityChange);



            yield break;
        }

        private void HandleOnDead(PlayerDeadType deadType)
        {
            Debug.Log("TEST - DDDDDDDDDDDDDDDDDDDDDD");
            switch (deadType)
            {
                case PlayerDeadType.KillerWind:
                    destroyed = true;

                    transform.root.GetComponent<Collider>().enabled = false;

                    // Destroy the balloon
                    // Activate all rigidbodies
                    rbBasket.isKinematic = false;
                    rbBoiler.isKinematic = false;
                    rbBalloon.isKinematic = false;
                    rbBalloon.useGravity = true;
                    rbBoiler.useGravity = true;
                    rbBasket.useGravity = true;

                    //Vector3 dir = GetRandomDir();

                    //rbBasket.AddForce(dir * 3, ForceMode.VelocityChange);
                    rbBasket.AddTorque(GetRandomDir(), ForceMode.VelocityChange);

                    //dir = GetRandomDir();
                    //rbBoiler.AddForce(dir * 3, ForceMode.VelocityChange);
                    //rbBoiler.AddTorque(dir, ForceMode.VelocityChange);

                    balloonForceDir = Vector3.up;
                    balloonTorqueDir = GetRandomDir();
                    
                    break;
            }
            
            
        }

        Vector3 GetRandomDir()
        {
            return (Random.Range(0, 2) == 0 ? Vector3.right : Vector3.left) * Random.Range(1f, 2f) +
                          (Random.Range(0, 2) == 0 ? Vector3.up : Vector3.down) * Random.Range(1f, 2f) +
                          (Random.Range(0, 2) == 0 ? Vector3.forward : Vector3.back) * Random.Range(1f, 2f);
        }
    }
}