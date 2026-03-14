using UnityEngine;

public class VerticalWind : MonoBehaviour
{
    [Header("Attivazione per Altezza")]
    public float startHeight = 20f;
    public float rampUpDistance = 10f;

    [Header("Parametri Oscillazione")]
    public float windSpeed = 0.5f;        // Più basso = più maestoso
    public float baseWindStrength = 0.2f;

    [Header("Mappa del Vento Globale")]
    public float globalWindMaxStrength = 1.5f;
    public float windMapScale = 0.001f;
    public float windEvolutionSpeed = 0.05f;

    [Header("Ammortizzatore (Smoothing)")]
    [Range(0.1f, 10f)]
    public float lerpSpeed = 2f; // Più basso è, più la mongolfiera è "pigra" e morbida

    private float lastAppliedOscillation = 0f;
    private float targetOscillation = 0f;

    void LateUpdate()
    {
        float h = transform.position.y;
        float posX = transform.position.x;
        float posZ = transform.position.z;

        float heightIntensity = Mathf.Clamp01((h - startHeight) / rampUpDistance);

        if (heightIntensity <= 0)
        {
            ResetOscillation();
            return;
        }

        // 1. Calcolo del valore "Target" (dove il vento vorrebbe portarci)
        float mapX = (posX * windMapScale) + (Time.time * windEvolutionSpeed);
        float mapZ = (posZ * windMapScale);
        float mapReading = Mathf.PerlinNoise(mapX, mapZ);

        float currentMaxStrength = baseWindStrength + (mapReading * globalWindMaxStrength);

        float timeNoise = Mathf.PerlinNoise(Time.time * windSpeed, 150f);
        targetOscillation = (timeNoise * 2f - 1f) * currentMaxStrength * heightIntensity;

        // 2. AMMORTIZZATORE (Smoothing)
        // Invece di saltare subito a targetOscillation, ci avviciniamo gradualmente
        float smoothedOscillation = Mathf.Lerp(lastAppliedOscillation, targetOscillation, Time.deltaTime * lerpSpeed);

        // 3. Applicazione Differenziale
        float delta = smoothedOscillation - lastAppliedOscillation;
        transform.position += new Vector3(0, delta, 0);

        // Memorizziamo il valore effettivamente applicato
        lastAppliedOscillation = smoothedOscillation;
    }

    private void ResetOscillation()
    {
        if (Mathf.Abs(lastAppliedOscillation) > 0.001f)
        {
            // Rientro morbido a zero anche quando si scende sotto la quota
            float smoothedReset = Mathf.Lerp(lastAppliedOscillation, 0f, Time.deltaTime * lerpSpeed);
            float delta = smoothedReset - lastAppliedOscillation;
            transform.position += new Vector3(0, delta, 0);
            lastAppliedOscillation = smoothedReset;
        }
        else
        {
            lastAppliedOscillation = 0f;
        }
    }
}