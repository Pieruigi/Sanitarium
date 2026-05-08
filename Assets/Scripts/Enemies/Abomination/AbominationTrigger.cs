using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.WSA;

namespace Baloon
{
    public class AbominationTrigger : MonoBehaviour
    {
        [SerializeField]
        Transform door;

        [SerializeField]
        GameObject _light;

        [SerializeField]
        Renderer lampRenderer;

        [SerializeField]
        Material lampOn, lampOff;

        [SerializeField]
        GameObject abomination;

        [SerializeField]
        Collider _collider;

        [SerializeField]
        AudioSource doorAudioSource;

        float openAngle = 160f;

        int lampMaterialIndex = 3;

        private void Awake()
        {
            _collider.enabled = false;
            _light.SetActive(false);
            var mats = lampRenderer.materials;
            mats[lampMaterialIndex] = lampOff;
            lampRenderer.materials = mats;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.X))
                Activated(); // Call this by an event 
#endif
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            // Open the door
            door.DOLocalRotate(Vector3.up * 160f, .5f).SetEase(Ease.OutBounce);

            // Play audio
            doorAudioSource.Play();

            // Camera shake
            CameraShake.Instance.PlayJumpscare(.5f);
        }

        void Activated()
        {
            // Activate light
            _light.SetActive(true);
            // Activate the collider
            _collider.enabled=true;
            // Set lamp material
            var mats = lampRenderer.materials;
            mats[lampMaterialIndex] = lampOn;
            lampRenderer.materials = mats;

        }
    }
}