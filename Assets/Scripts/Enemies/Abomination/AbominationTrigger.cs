using Baloon.SaveSystem;
using DG.Tweening;
using StarterAssets;
using System.Collections;
using UnityEngine;


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

        [SerializeField]
        GameObject normalShelf, collapsedShelf;

        [SerializeField]
        Transform exitGate, exitDoor;

        [SerializeField]
        BlooderController blooder;

        float openAngle = 160f;

        int lampMaterialIndex = 3;

        FirstPersonController player;

        bool triggered = false;

        [SerializeField]
        string saveId;

        class Data
        {
            public bool enabled;
            public bool triggered;
        }


        private void Awake()
        {

            _collider.enabled = false;
            _light.SetActive(false);
            var mats = lampRenderer.materials;
            mats[lampMaterialIndex] = lampOff;
            lampRenderer.materials = mats;
            collapsedShelf.SetActive(false);

#if UNITY_EDITOR
            //_collider.enabled = true;
#endif
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = FindFirstObjectByType<FirstPersonController>();

            // Load data
            var jsonData =  SaveManager.Instance.GetRawJsonData(saveId);
            if (!string.IsNullOrEmpty(jsonData))
            {
                var data = JsonUtility.FromJson<Data>(jsonData);

                _collider.enabled = data.enabled;
                triggered = data.triggered;
                if(data.enabled || data.triggered)
                {
                    // Enable or already triggered
                    EnableObjects();

                    if (data.triggered)
                    {
                        door.DOLocalRotate(Vector3.up * 160f, .5f).SetEase(Ease.OutBounce);
                        // Destroy abomination
                        Destroy(abomination);
                    }                    
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //    Activated(); // Call this by an event 
#endif
        }

        private void OnEnable()
        {
            BlooderController.OnSealed += HandleOnSealed;
            SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
        }

        private void OnDisable()
        {
            BlooderController.OnSealed -= HandleOnSealed;
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
        }

        private void HandleOnUpdateDataEntry()
        {
            var data = new Data();
            data.enabled = _collider.enabled;
            data.triggered = triggered;
            SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
        }

        private void HandleOnSealed(BlooderController blooderController)
        {
            if (blooder != blooderController) return;

            Activated();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            // Reset collider 
            _collider.enabled = false;

            triggered = true;

            // Open the door
            door.DOLocalRotate(Vector3.up * 160f, .5f).SetEase(Ease.OutBounce);

            // Play audio
            doorAudioSource.Play();

            //StartCoroutine(DisablePlayerForAWhile());
            player.DisableAndLookForSeconds(abomination.transform.position);

            // Camera shake
            CameraShake.Instance.PlayJumpscare(.5f);

            // Look at the creature
            var dir = Vector3.ProjectOnPlane(abomination.transform.position - player.transform.position, Vector3.up);
            player.transform.forward = dir;
            player.ForceCameraPitch(0f);

            // FOV
            FOVController.Instance.JumpscareFOV(20f, .5f);

            // Jumpscare
            AudioManager.Instance.PlayJumpscare();

            // Wake up abomination
            abomination.GetComponent<AbominationController>().StartChasingPlayer();

            
        }

        void Activated()
        {
            
            // Activate the collider
            _collider.enabled=true;
            triggered = false;
            
            EnableObjects();
        }

        void EnableObjects()
        {
            // Activate light
            _light.SetActive(true);
            // Set lamp material
            var mats = lampRenderer.materials;
            mats[lampMaterialIndex] = lampOn;
            lampRenderer.materials = mats;
            // Block path
            collapsedShelf.SetActive(true);
            normalShelf.SetActive(false);
            // Open exit gate and door
            exitDoor.transform.localEulerAngles = Vector3.up * -21f;
            exitGate.transform.localEulerAngles = Vector3.up * -138f;
        }
    }
}