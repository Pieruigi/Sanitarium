using StarterAssets;
using System.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

namespace Baloon
{
    public class ChonchonTrigger : MonoBehaviour
    {
        [SerializeField]
        Transform target;

        [SerializeField]
        GameObject prefab;

        bool spawned = false;

        GameObject creature;

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
            if (spawned) return;

            StartCoroutine(DoSpawn(other.GetComponent<FirstPersonController>()));

            //StartCoroutine(DoJumpscare(other.GetComponent<FirstPersonController>()));

            IEnumerator DoSpawn(FirstPersonController player)
            {
                spawned = true;

                player.DisableAndLookForSeconds(target.position);

                CameraShake.Instance.PlayJumpscare(1f);

                yield return new WaitForSeconds(.1f);

                creature = Instantiate(prefab, target);
            }

            //IEnumerator DoJumpscare(FirstPersonController player)
            //{
            //    player.DisableAndLookForSeconds(target.position);
                
            //    //player.JawDisabled = true;
            //    //player.PitchDisabled = true;
            //    //player.MoveDisabled = true;

            //    //var dir = target.position - player.transform.position;

            //    //player.ForceRotation(Quaternion.LookRotation(dir.normalized, Vector3.up));
            //    //player.ForceCameraPitch(0);

            //    CameraShake.Instance.PlayJumpscare(1f);

            //    //yield return new WaitForSeconds(.5f);
            //    //player.JawDisabled = false;
            //    //player.PitchDisabled = false;
            //    //player.MoveDisabled = false;
            //}
        }
    }
}