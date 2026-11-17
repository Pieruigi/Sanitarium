using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.UIElements;


namespace Baloon
{
    public class PathTileLightChecker : MonoBehaviour
    {
        
        float playerDistance = 4f;

        GameObject player;

        PathTile tile;
                

        private void Awake()
        {
            tile = GetComponentInParent<PathTile>();
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
            if (!tile.IsShaking) return;
            
            if (Vector3.Distance(transform.position, player.transform.position) < playerDistance)
            {
                if (Flashlight.Instance.IsOn)
                {
                    // Raycast
                    if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 2*playerDistance))
                    {
                        if(hit.collider.gameObject == gameObject && tile.Walkable)
                            tile.StopShaking();
                    }
                }
                

            }
            

            
        }
    }

}
