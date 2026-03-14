using UnityEngine;

public class VerticalWind : MonoBehaviour
{
    [Header("Attivazione per Altezza")]
    public float startHeight = 20f;      // Altezza a cui inizia il vento
    public float rampUpDistance = 10f;   // Distanza per raggiungere l'intensità piena

    [Header("Parametri Oscillazione")]
    public float windSpeed = 1.2f;       // Quanto è frenetica l'oscillazione
    public float baseWindStrength = 0.5f; // Turbolenza minima sempre presente

    [Header("Mappa del Vento Globale (Noise 2D)")]
    public float globalWindMaxStrength = 4f; // Forza extra nelle zone di "tempesta"
    public float windMapScale = 0.005f;      // Dimensione delle zone (piccolo = zone grandi)
    public float windEvolutionSpeed = 0.1f;  // Velocità con cui le zone di vento si spostano

    private float lastOscillation = 0f;

    void LateUpdate()
    {
        // 1. Coordinate attuali
        float h = transform.position.y;
        float posX = transform.position.x;
        float posZ = transform.position.z;

        // 2. Calcolo intensità in base all'altezza (Ramp-up)
        // Se h < startHeight, intensity sarà 0.
        float heightIntensity = Mathf.Clamp01((h - startHeight) / rampUpDistance);

        if (heightIntensity <= 0)
        {
            ResetOscillation();
            return;
        }

        // 3. Lettura della Mappa del Vento (Noise 2D)
        // Usiamo X e Z per campionare il rumore. Aggiungiamo il tempo per far muovere le zone.
        float mapX = (posX * windMapScale) + (Time.time * windEvolutionSpeed);
        float mapZ = (posZ * windMapScale);
        float mapReading = Mathf.PerlinNoise(mapX, mapZ);

        // Forza massima potenziale in questo punto esatto della mappa
        float currentMaxStrength = baseWindStrength + (mapReading * globalWindMaxStrength);

        // 4. Calcolo dell'oscillazione temporale (Il "respiro" del vento)
        // Usiamo un seme diverso o coordinate diverse per non sovrapporci alla mappa
        float timeNoise = Mathf.PerlinNoise(Time.time * windSpeed, 150f);
        float currentOscillation = (timeNoise * 2f - 1f) * currentMaxStrength * heightIntensity;

        // 5. Applicazione Differenziale (Delta)
        // Spostiamo il transform solo della differenza rispetto al frame precedente
        float delta = currentOscillation - lastOscillation;
        transform.position += new Vector3(0, delta, 0);

        // Memorizziamo per il prossimo frame
        lastOscillation = currentOscillation;
    }

    private void ResetOscillation()
    {
        // Se scendiamo sotto la quota del vento, azzeriamo gradualmente l'offset
        if (lastOscillation != 0)
        {
            transform.position -= new Vector3(0, lastOscillation, 0);
            lastOscillation = 0;
        }
    }
}