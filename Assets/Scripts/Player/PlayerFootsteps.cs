using StarterAssets;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;


namespace Baloon
{
    public enum SurfaceType { Concrete, Metal }

    public class PlayerFootsteps : MonoBehaviour
    {

        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        List<AudioClip> concreteAudioClips, metalAudioClips;

        float concreteVolume = 0.1f;
        float metalVolume = .04f;

        //SurfaceType footstepTypeDefault = SurfaceType.Concrete;

        FirstPersonController player;

        float baseTime = 2f;

        float currentTime;
        float elapsed;

        private void Awake()
        {
            player = GetComponent<FirstPersonController>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if (player.GetSpeed() > 0)
            {
                currentTime = baseTime / player.GetSpeed();

                elapsed += Time.deltaTime;
                if (elapsed > currentTime)
                {
                    elapsed -= currentTime;

                    // Get surface
                    var type = GetSurfaceType();

                    switch (type)
                    {
                        case SurfaceType.Metal:
                            audioSource.clip = metalAudioClips[Random.Range(0, metalAudioClips.Count)];
                            audioSource.volume = metalVolume;
                            break;
                        default:
                            audioSource.clip = concreteAudioClips[Random.Range(0, concreteAudioClips.Count)];
                            audioSource.volume = concreteVolume;
                            break;
                    }

                    audioSource.Play();

                }
            }
            else
            {
                elapsed = baseTime;
            }
        }

        SurfaceType GetSurfaceType()
        {
            // Raycast
            var mask = LayerMask.GetMask(new string[] { "Ground" , "SurfaceData" });
            //mask = -1;
            var origin = transform.position + Vector3.up * .5f;
            var direction = Vector3.down;
            var distance = .6f;
            RaycastHit hit;
            SurfaceTypeData data = null;
            if (Physics.Raycast(origin, direction, out hit, distance, mask, QueryTriggerInteraction.Collide))
                data = hit.collider.GetComponent<SurfaceTypeData>();
            
            if(data != null)
                return data.Type;
            else
                return SurfaceType.Concrete; // Default
        }
    }
}