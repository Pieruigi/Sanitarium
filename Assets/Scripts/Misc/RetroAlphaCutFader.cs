using UnityEngine;

public class RetroAlphaCutFader : MonoBehaviour
{
    [Header("Shader Settings")]
    public string cutoffPropertyName = "_Cutoff"; // Nome standard URP/RetroShader
    public int materialIndex = 0;

    [Header("Distance Settings")]
    public float minDistance = 5f;
    public float maxDistance = 30f;

    private Material _targetMaterial;
    private Transform _cam;

    [SerializeField]
    bool useAlpha = false;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null && materialIndex < rend.materials.Length)
        {
            // Usiamo l'istanza come prima per sicurezza con RetroShader Pro
            _targetMaterial = rend.materials[materialIndex];
        }
        _cam = Camera.main.transform;
    }

    void Update()
    {
        if (_targetMaterial == null || _cam == null) return;

        float dist = Vector3.Distance(transform.position, _cam.position);

        // Calcoliamo il cutoff: 
        // Vicino (minDistance) -> Cutoff 0 (tutto visibile)
        // Lontano (maxDistance) -> Cutoff 1 (tutto invisibile)
        //float cutoff = Mathf.Clamp01((dist - minDistance) / (maxDistance - minDistance));
        // Se fosse al contrario (Appare lontano, sparisce vicino):
        float cutoff = Mathf.Clamp01((dist - minDistance) / (maxDistance - minDistance));

        if(!useAlpha)
        {
            _targetMaterial.SetFloat(cutoffPropertyName, cutoff);
        }
        else
        {
            var v = _targetMaterial.GetVector(cutoffPropertyName);
            _targetMaterial.SetVector(cutoffPropertyName, new Vector4(v.x,v.y,v.z,(1f- cutoff)));
        }
            
    }
}