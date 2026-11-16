using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.UIElements;


namespace Baloon
{
    public class PathTileLightChecker : MonoBehaviour
    {
        
        float playerDistance = 3f;

        GameObject player;

        bool isOn = false;

        Renderer _renderer;

                

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _renderer.enabled = false;

           
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = FindFirstObjectByType<FirstPersonController>().gameObject;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void LateUpdate()
        {
            bool isOnOld = isOn;
            if (Vector3.Distance(transform.position, player.transform.position) < playerDistance)
            {
                if (Flashlight.Instance.IsOn)
                {
                    isOn = true;
                }
                else
                {
                    isOn = false;
                }

            }
            else
            {
                isOn = false;
            }

            if(isOnOld != isOn) // Light on/off
            {
                Debug.Log("TEST - Path - Light check:" + isOn);
                

               
                _renderer.enabled = isOn;
            }
        }
    }

}
