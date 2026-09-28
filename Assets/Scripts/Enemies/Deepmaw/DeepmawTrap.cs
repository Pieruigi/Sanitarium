using DG.Tweening;
using StarterAssets;
using System.Collections;
using UnityEngine;

namespace Baloon
{
    public class DeepmawTrap : MonoBehaviour
    {
        [SerializeField]
        GameObject deepmaw;

        [SerializeField]
        AudioSource goreAudioSource;

        bool killing = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || killing) return;

            killing= true;

            KillPlayer();

        }

        void KillPlayer()
        {

            StartCoroutine(DoKill());

            IEnumerator DoKill()
            {
                // Stop player input and look the abomination direction
                FirstPersonController player = FindFirstObjectByType<FirstPersonController>(); //target.GetComponent<FirstPersonController>();
                player.Doomed = true;
                player.JawDisabled = true;
                player.PitchDisabled = true;
                player.MoveDisabled = true;


                // Disable flashlight
                Flashlight.Instance.gameObject.SetActive(false);

                // Set creature animation
                var animator = deepmaw.GetComponentInChildren<Animator>();
                animator.SetTrigger("Attack");

                // Set the creature as camera child
                deepmaw.transform.parent = Camera.main.transform;

                // Set position and rotation
                var targetPos = new Vector3(0.0469999984f, -0.0839999989f, 0.954999983f);//  Vector3.zero;
                var targetRot = Vector3.up * 180;

                float duration = .1f;
                deepmaw.transform.localPosition = targetPos;
                deepmaw.transform.localEulerAngles = targetRot;
                //deepmaw.transform.DOLocalMove(targetPos, duration);
                //deepmaw.transform.DOLocalRotate(targetRot, duration);


                //// Move the creature to the player camera
                //var targetPos = Camera.main.transform.position;// + Camera.main.transform.forward * 1f;
                //var targetFwd = -Camera.main.transform.forward;

                //var targetRot = Quaternion.LookRotation(targetFwd, Camera.main.transform.up);

                //var localRotationOffset = Vector3.zero;
                //if (localRotationOffset != Vector3.zero)
                //{
                //    targetRot *= Quaternion.Euler(localRotationOffset);
                //}

                //var duration = .1f;
                //deepmaw.transform.DORotateQuaternion(targetRot, duration);
                //deepmaw.transform.DOMove(targetPos, duration);

                deepmaw.transform.DOShakePosition(1.5f, strength: .15f);

                // Jumpscare
                CameraShake.Instance.PlayJumpscare(1.5f);

                // Play jumspcare audio
                AudioManager.Instance.PlayJumpscare();

                // Play gore delayed
                goreAudioSource.PlayDelayed(1.5f);

                yield return new WaitForSeconds(1.5f);

                player.Die(PlayerDeadType.CreatureAttack);

                yield break;
            }
        }
        
    }
}