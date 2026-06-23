using Baloon;
using Baloon.SaveSystem;
using UnityEngine;

public class CatwalkCollapserTrigger : MonoBehaviour
{
    [SerializeField]
    bool enableOnStart = false;

    [SerializeField]
    string saveId;

    [SerializeField]
    BlooderController blooder; // Can be null

    [SerializeField]
    CatwalkCollapser collapser;

    Collider _collider;

    class Data
    {
        public bool activated;
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        if(!enableOnStart)
            _collider.enabled = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string rawData = SaveManager.Instance.GetRawJsonData(saveId);
        if (!string.IsNullOrEmpty(rawData))
        {
            var data = JsonUtility.FromJson<Data>(rawData); 
            _collider.enabled = data.activated;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
        BlooderController.OnSealed += HandleOnBlooderSealed;
    }

    private void OnDisable()
    {
        SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
        BlooderController.OnSealed -= HandleOnBlooderSealed;
    }

    

    private void HandleOnBlooderSealed(BlooderController blooderController)
    {
        if (blooderController != blooder) return;

        _collider.enabled = true;
    }

    private void HandleOnUpdateDataEntry()
    {
        var data = new Data();
        data.activated = _collider.enabled;
        SaveManager.Instance.CreateOrUpdateDataEntry(saveId, JsonUtility.ToJson(data));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        _collider.enabled = false;
        collapser.Play();
    }
}
