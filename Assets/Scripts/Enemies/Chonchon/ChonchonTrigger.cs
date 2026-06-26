using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class ChonchonTrigger : MonoBehaviour
    {
        [SerializeField]
        Transform target;

        [SerializeField]
        GameObject prefab;

        [SerializeField]
        List<Rigidbody> rocks;

        [SerializeField]
        Transform explosionTarget;

        [SerializeField]
        Transform idleTarget;

        [SerializeField]
        AudioSource explosionAudioSource;

        bool spawned = false;

        GameObject creature;

        private void Awake()
        {
            foreach(var rock in rocks)
            {
                rock.isKinematic = true;
            }
        }

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
            if (!spawned)
            {
                StartCoroutine(DoSpawn(other.GetComponent<FirstPersonController>()));

                //StartCoroutine(DoJumpscare(other.GetComponent<FirstPersonController>()));

                IEnumerator DoSpawn(FirstPersonController player)
                {
                    spawned = true;

                    explosionAudioSource.Play();

                    // Apply explosion force
                    foreach (var rock in rocks)
                    {
                        rock.isKinematic = false;
                        rock.AddExplosionForce(1000f, explosionTarget.position, 20f);
                        rock.AddTorque(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
                    }

                    player.DisableAndLookForSeconds(target.position);

                    CameraShake.Instance.PlayJumpscare(1f);

                    yield return new WaitForSeconds(.1f);

                    creature = Instantiate(prefab, target.position, target.rotation);
                    creature.GetComponent<ChonchonController>().SetIdleTarget(idleTarget);
                }
            }
            else
            {
                creature.GetComponent<ChonchonController>().UnsetIdleState();
            }
            

        }
    }
}