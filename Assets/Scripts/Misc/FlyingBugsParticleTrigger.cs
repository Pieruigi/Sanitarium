using UnityEngine;

namespace Baloon
{
    public class FlyingBugsParticleTrigger : MonoBehaviour
    {
        [SerializeField]
        GameObject prefab;

        float lifeTime = 12;

        bool triggered = false;

        GameObject _object;

        float time = 0;

        float yOffset = -7;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.X))
            {
                Spawn();
            }
#endif
        }

        private void LateUpdate()
        {
            if (_object)
            {
                // Adjust position
                var pos = BaloonController.Instance.transform.position;
                pos.y += yOffset;
                _object.transform.position = pos;

                // Check lifetime
                time += Time.deltaTime;

                if(time > lifeTime)
                {
                    Destroy(_object);
                    _object = null;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggered || !other.CompareTag("Player")) return;

            triggered = true;

            Spawn();
        }

        void Spawn()
        {
            // Set position
            _object = Instantiate(prefab);
            var pos = BaloonController.Instance.transform.position;
            pos.y += yOffset;
            _object.transform.position = pos;

            time = 0;

            CameraShake.Instance.PlayJumpscare(8f);
        }

    }

}
