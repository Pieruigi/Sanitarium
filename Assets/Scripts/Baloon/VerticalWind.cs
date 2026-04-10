#define NEW_VERTICAL_WIND
#if NEW_VERTICAL_WIND
using Baloon;
using UnityEngine;

public class VerticalWind : Singleton<VerticalWind>
{
    [Header("Wind Settings")]
    [Tooltip("General strength of the vertical wind")]
    public float windStrength = 0.5f;

    [SerializeField]
    public float windStrengthMax = 1.5f;
    
    [Tooltip("How fast the wind changes intensity and direction")]
    public float changeSpeed = 0.2f;

    [Header("Debug Info")]
    [SerializeField] private float currentVerticalForce;
    private float seed;

    bool running = false;

    float baseStrength;

    public bool Running
    {
        get { return running; }
        set { running = value; }
    }

    protected override void Awake()
    {
        base.Awake();

        baseStrength = windStrength;
    }

    void Start()
    {
        // Random seed so every game session has different wind patterns
        seed = Random.Range(0f, 1000f);
    }

    void Update()
    {
        if (!running) return;

        ApplyVerticalWind();
    }

    private void OnEnable()
    {
        BasePlatform.OnLanding += HandleOnLanding;
        BasePlatform.OnTakeOff += HandleOnTakeOf;
    }

    private void OnDisable()
    {
        BasePlatform.OnLanding -= HandleOnLanding;
        BasePlatform.OnTakeOff -= HandleOnTakeOf;
    }

    private void HandleOnLanding(BasePlatform platform)
    {
        running = false;
    }

    private void HandleOnTakeOf(BasePlatform platform)
    {
        running = true;
    }

    private void ApplyVerticalWind()
    {
        AdjustWindStrength(BaloonController.Instance.Altitude);

        // We use PerlinNoise to get a value between 0 and 1
        // Multiplying Time.time by changeSpeed dictates how fast the "curve" moves
        float noise = Mathf.PerlinNoise(seed + Time.time * changeSpeed, 0);

        // Map the 0...1 noise to -1...1 to get both UP and DOWN directions
        float directionFactor = (noise - 0.5f) * 2f;

        // Final force calculation
        currentVerticalForce = directionFactor * windStrength;

        // Apply the movement to the balloon
        // Using Translate or adding to your physics controller
        transform.Translate(Vector3.up * currentVerticalForce * Time.deltaTime);
    }

    void AdjustWindStrength(float currentAltitude)
    {
        const float minH = 20f;
        const float maxH = 120f;
        float minW = baseStrength;
        float maxW = windStrengthMax;
        // 1. Calculate the rate of change (Wind units per Meter)
        // In this case: (1.5 - 0.5) / (120 - 30) = 1.0 / 90 = ~0.011
        float windPerMeter = (maxW - minW) / (maxH - minH);

        // 2. Apply the slope starting from the base point (30m, 0.5W)
        // This works for 10m, 200m, or any value.
        windStrength = minW + (currentAltitude - minH) * windPerMeter;

        // Optional: Safety check to avoid negative wind if altitude goes below zero
        if (windStrength < 0) windStrength = 0;
    }

    // Public method to check the wind force from other scripts (like the Manifold/Throttle)
    public float GetCurrentWindForce()
    {
        return currentVerticalForce;
    }


}
#else
using UnityEngine;

public class VerticalWind : MonoBehaviour
{
    [Header("Attivazione per Altezza")]
    public float startHeight = 0f;
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

        if (heightIntensity <= 0 || Baloon.BasePlatform.CurrentPlatform)
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
#endif