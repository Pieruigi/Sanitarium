using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


namespace Baloon
{
    public class BalloonCollisionChecker : MonoBehaviour
    {
        [SerializeField]
        AudioSource collisionAudioSource;

        FirstPersonController player;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var collider = GetComponent<Collider>();
            var rb = collider.GetComponent<Rigidbody>();
            

            // Remove player collision
            player = GameObject.FindFirstObjectByType<FirstPersonController>();

            Physics.IgnoreCollision(collider, player.GetComponent<Collider>(), true);

            // Disable balloon internal collisions
            var root = GetComponentInParent<BaloonController>().gameObject;
            var others = root.GetComponentsInChildren<Collider>().Where(c=>c != collider);
            foreach (var other in others)
                Physics.IgnoreCollision(collider, other, true);

            // Disable platform collisions
            //var platforms = FindObjectsByType<BasePlatform>(FindObjectsSortMode.None);
            //foreach(var other in platforms)
            //    Physics.IgnoreCollision(collider, other.GetComponent<Collider>(), true);


        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (BasePlatform.CurrentPlatform || player.Doomed) return;

            //player.Doomed = true;

            // Store current velocity
            var vel = BaloonController.Instance.CurrentVelocity;

            // Get normal
            var normal = collision.contacts[0].normal;

            //float horizontalMagnitude = new Vector2(vel.x, vel.z).magnitude;

            // Get the absolute value of the vertical component
            //float verticalMagnitude = Mathf.Abs(vel.y);

            Vector3 bounceVel = Vector3.zero;

            //bool isPointingUpMoreThan45 = normal.y > 0.7071f;

            var hV = new Vector3(vel.x, 0f, vel.z);
            var hN = new Vector3(normal.x, 0f, normal.z);

            var hDot = Vector3.Dot(hV, hN);

            if(hDot > 0)
            {
                bounceVel.x = vel.x * 1f;
                bounceVel.z = vel.z * 1f;
            }
            else
            {
                bounceVel.x = vel.x * -1.75f;
                bounceVel.z = vel.z * -1.75f;
            }

            if(normal.y > 0 && vel.y < 0)
            {
                // Invert vertical velocity
                bounceVel.y = -vel.y;
                
            }
            else
            {
                bounceVel.y = vel.y;
            }




                //if (horizontalMagnitude > verticalMagnitude) // Horizontal
                //{
                //    bounceVel = vel;
                //    bounceVel.x *= -1.75f;
                //    bounceVel.z *= -1.75f;
                //}
                //else // Vertical
                //{
                //    bounceVel = vel;
                //    bounceVel *= -1.75f;
                //}

                // Stop moving forward
                //BaloonController.Instance.DisableHorizontalVelocity();

            // Move back
            var time = 2f;
            //vel.y *= -1;
            var bounceTarget = BaloonController.Instance.transform.position + bounceVel * time;
            BaloonController.Instance.transform.DOMove(bounceTarget, time).SetEase(Ease.OutQuad);
            CameraShake.Instance.PlayKillerWindShake(time);
            BaloonShaker.Instance.StartWarningShake(time);

            // Play audio
            collisionAudioSource.Play();

            //StartCoroutine(GetComponentInParent<BaloonDestroyer>().DoPlayExplosion(time));
            StartCoroutine(TakeDamage());


            IEnumerator TakeDamage()
            {
                yield return new WaitForSeconds(1.5f);

                if (player.Doomed) yield break;

                BaloonBoilerHealth.Instance.TryTakeSingleDamage();
            }

        }

        private void __OnCollisionEnter(Collision collision)
        {
            if (BasePlatform.CurrentPlatform || player.Doomed) return;

            //player.Doomed = true;

            // Store current velocity
            var vel = BaloonController.Instance.CurrentVelocity;

            float horizontalMagnitude = new Vector2(vel.x, vel.z).magnitude;

            // Get the absolute value of the vertical component
            float verticalMagnitude = Mathf.Abs(vel.y);

            Vector3 bounceVel = Vector3.zero;

            if (horizontalMagnitude > verticalMagnitude) // Horizontal
            {
                bounceVel = vel;
                bounceVel.x *= -1.75f;
                bounceVel.z *= -1.75f;
            }
            else // Vertical
            {
                bounceVel = vel;
                bounceVel *= -1.75f;
            }

           // Stop moving forward
           //BaloonController.Instance.DisableHorizontalVelocity();

            // Move back
            var time = 2f;
            //vel.y *= -1;
            var bounceTarget = BaloonController.Instance.transform.position + bounceVel * time;
            BaloonController.Instance.transform.DOMove(bounceTarget, time).SetEase(Ease.OutQuad);
            CameraShake.Instance.PlayKillerWindShake(time);
            BaloonShaker.Instance.StartWarningShake(time);

            // Play audio
            collisionAudioSource.Play();

            //StartCoroutine(GetComponentInParent<BaloonDestroyer>().DoPlayExplosion(time));
            StartCoroutine(TakeDamage());


            IEnumerator TakeDamage()
            {
                yield return new WaitForSeconds(1.5f);

                if (player.Doomed) yield break;

                BaloonBoilerHealth.Instance.TryTakeSingleDamage();
            }

        }

        private void _OnCollisionEnter(Collision collision)
        {
          
            if (BasePlatform.CurrentPlatform || player.Doomed) return;

            player.Doomed = true;

            // Store current velocity
            var vel = BaloonController.Instance.CurrentVelocity;

            // Stop moving forward
            BaloonController.Instance.DisableHorizontalVelocity();

            // Move back
            var time = 1.5f;
            vel.y = 0;
            var bounceTarget = BaloonController.Instance.transform.position - vel * time;
            BaloonController.Instance.transform.DOMove(bounceTarget, time).SetEase(Ease.OutQuad);
            CameraShake.Instance.PlayKillerWindShake(time);
            BaloonShaker.Instance.StartWarningShake(time);

            // Play audio
            collisionAudioSource.Play();

            StartCoroutine(GetComponentInParent<BaloonDestroyer>().DoPlayExplosion(time));

            
            
        }

    }
}