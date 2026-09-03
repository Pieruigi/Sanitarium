using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Baloon
{
    public class PathPuzzleController : MonoBehaviour
    {
        [SerializeField]
        List<PathTile> tiles;

        [SerializeField]
        GameObject monsterPrefab;

        [SerializeField]
        AudioSource rumblingAudioSource;

        [SerializeField]
        AudioSource fallingAudioSource;

        bool inside = false;

        GameObject player;

        bool collapsing = false;

        GameObject monster;

        bool dead = false;

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

        private void LateUpdate()
        {
            if (!monster || dead) return;

            var pos = monster.transform.position;
            pos.x = player.transform.position.x;
            pos.z = player.transform.position.z;

            monster.transform.position = pos;

            if (Vector3.Distance(player.transform.position, monster.transform.position) < 1f)
            {
                dead = true;
                player.GetComponent<FirstPersonController>().Die(PlayerDeadType.CreatureAttack);
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

            rumblingAudioSource.Stop();
            fallingAudioSource.Play();

            StartCoroutine(DoStopPlayer());

            SpawnMonster();

            IEnumerator DoStopPlayer()
            {

                FirstPersonController fpc = player.GetComponent<FirstPersonController>();
                fpc.Gravity = 64f;
                
                //fpc.MoveDisabled = false;
                fpc.Doomed = true;

                CameraShake.Instance.PlayJumpscare(.5f);

                float time = .5f;
                while (time > 0)
                {
                    time -= Time.deltaTime;
                    yield return null;
                }

                fpc.Gravity = -600f;
                //fpc.MoveDisabled = true;

                yield return new WaitForSeconds(4f);

                if (!dead)
                {
                    dead = true;
                    player.GetComponent<FirstPersonController>().Die(PlayerDeadType.CreatureAttack);
                }
                
                //fpc.GetComponent<CharacterController>().center = Vector3.up * 10000;
                //fpc.GroundCheckDisabled = true;
                //FindFirstObjectByType<Terrain>().GetComponent<TerrainCollider>().enabled = false;
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

        void SpawnMonster()
        {
            monster = Instantiate(monsterPrefab);
            monster.transform.position = player.transform.position;
            monster.transform.Translate(Vector3.down * 30f);
            monster.transform.eulerAngles = Vector3.zero;
        }
    }
}