using UnityEngine;

public class DistanceScaleFader : MonoBehaviour
{
    [Header("Distance Settings")]
    public float minDistance = 5f;  // Distanza a cui l'oggetto ha la scala piena
    public float maxDistance = 30f; // Distanza a cui l'oggetto scompare (scala 0)

    private Vector3 _initialScale;
    private Transform _mainCam;

    void Start()
    {
        // Memorizziamo la scala originale impostata nell'Inspector
        _initialScale = transform.localScale;

        if (Camera.main != null)
            _mainCam = Camera.main.transform;
    }

    void Update()
    {
        if (_mainCam == null) return;

        float distance = Vector3.Distance(transform.position, _mainCam.position);

        // Calcoliamo il fattore di scala: 1 (vicino) -> 0 (lontano)
        float scaleFactor = Mathf.Clamp01(1.0f - ((distance - minDistance) / (maxDistance - minDistance)));

        // Applichiamo la scala proporzionalmente alla scala iniziale
        transform.localScale = _initialScale * scaleFactor;

        // Ottimizzazione PS1: se l'oggetto è troppo piccolo per essere visto, 
        // potresti voler disattivare il renderer, ma la scala 0 è già molto efficiente.
    }
}