using Mono.Cecil.Cil;
using NUnit.Framework;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Baloon
{
    public class PathPuzzleController : MonoBehaviour
    {
        [SerializeField]
        List<PathTile> tiles;

        bool inside = false;

        GameObject player;

        bool collapsing = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {
            if (!inside || collapsing) return;

            // Raycast
            var origin = player.transform.position + Vector3.up * .2f;
            RaycastHit hit;
            if(Physics.Raycast(origin, Vector3.down, out hit, 3f))
            {
                var tile = hit.collider.GetComponent<PathTile>();
                if(tile != null)
                {
                    if (!tile.Walkable)
                        CollapseAll();
                }
            }
        }

        private void CollapseAll()
        {
            if (collapsing) return;
            collapsing = true;

            foreach (var tile in tiles)
            {
                //var rb = tile.AddComponent<Rigidbody>();
                //rb.useGravity = true;
                //rb.isKinematic = true;
                //rb.mass = 1f;
                tile.Fall();
            }

            StartCoroutine(DoStopPlayer());

            IEnumerator DoStopPlayer()
            {
                FirstPersonController fpc = player.GetComponent<FirstPersonController>();
                fpc.Gravity = 64f;
                
                //fpc.MoveDisabled = false;

                
                float time = 1f;
                while (time > 0)
                {
                    time -= Time.deltaTime;
                    yield return null;
                }

                fpc.Gravity = -600f;
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            inside = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            inside = false;
        }
    }
}