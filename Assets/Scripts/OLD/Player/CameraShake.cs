using UnityEngine;
using DG.Tweening;
using StarterAssets;

public class CameraShake : Singleton<CameraShake>
{
    private Vector3 originalPos;
    private Vector3 originalRot;

    private Tween shakeTween;
    private Tween rotTween;

    protected override void Awake()
    {
        base.Awake();
        originalPos = transform.localPosition;
        originalRot = transform.localEulerAngles;
    }

#if UNITY_EDITOR
    private void Update()
    {
      
    }
#endif

    // -----------------------------
    // PUBLIC METHODS
    // -----------------------------
    public void PlayVerticalWindShake(float duration, System.Action onComplete = null, System.Action onKill = null)
    {
        float randDuration = duration;// Random.Range(3.2f * 2f, 4.0f * 2f);
        float randPos = Random.Range(0.006f*2f, 0.009f*2f);
        float randRot = Random.Range(0.02f*2f, 0.04f*2f);

        PlayShake(
            duration: randDuration,
            posStrength: randPos,
            rotStrength: randRot,
            vibratoPos: 0,
            vibratoRot: 0,
            onComplete, 
            onKill
        );

    }


    /// <summary>
    /// Stato VERDE: Altezza perfetta. 
    /// Un dondolio leggero quasi impercettibile, per dare vita all'ambiente.
    /// </summary>
    public void PlayWindShakeLight(System.Action onComplete = null, System.Action onKill = null)
    {
        float randDuration = Random.Range(3.2f, 4.0f);
        float randPos = Random.Range(0.012f, 0.018f);
        float randRot = Random.Range(0.4f, 0.8f);

        PlayShake(
            duration: randDuration,
            posStrength: randPos,
            rotStrength: randRot,
            vibratoPos: 1,
            vibratoRot: 1,
            onComplete
        );
        
    }

 

    public void PlayWindShakeStrong(System.Action onComplete = null, System.Action onKill = null)
    {
        float randDuration = Random.Range(3.2f, 4.0f);
        float randPos = Random.Range(0.024f, 0.036f);
        float randRot = Random.Range(0.8f, 1.6f);
        // Qui il vento "schiaffeggia" la mongolfiera. 
        // Spostamento molto forte, ma sempre fluido grazie al vibrato 1.
        PlayShake(
            duration: randDuration,
            posStrength: randPos,
            rotStrength: randRot,
            vibratoPos: 2,
            vibratoRot: 2,
            onComplete, 
            onKill
        );
    }

    public void PlayWindGustShake(float duration, System.Action onComplete = null, System.Action onKill = null)
    {
        Debug.Log("TEST - Wind gust shake");

        float randPos = Random.Range(0.03f, 0.04f);
        float randRot = Random.Range(1.5f, 2.5f);
        // Qui il vento "schiaffeggia" la mongolfiera. 
        // Spostamento molto forte, ma sempre fluido grazie al vibrato 1.
        PlayShake(
            duration: duration,
            posStrength: randPos,
            rotStrength: randRot,
            vibratoPos: 4,
            vibratoRot: 4,
            onComplete, 
            onKill
        );
    }

    public void PlayKillerWindShake(float duration)
    {
        float randPos = Random.Range(0.3f, 0.4f);
        float randRot = Random.Range(2f, 2.85f);
        // Qui il vento "schiaffeggia" la mongolfiera. 
        // Spostamento molto forte, ma sempre fluido grazie al vibrato 1.
        PlayShake(
            duration: duration,
            posStrength: randPos,
            rotStrength: randRot,
            vibratoPos: 10,
            vibratoRot: 10
            
        );
    }

    public void PlayJumpscare()
    {
        //var fpc = FindFirstObjectByType<FirstPersonController>();
        //fpc.InputDisabled = true;

        PlayShake(
            duration: 1.7f,
            posStrength: 0.25f * .4f,
            rotStrength: 15f * .4f,
            vibratoPos: 30,
            vibratoRot: 20
            // onComplete: () =>
            // {
            //     fpc.InputDisabled = false;
            // }
        );
    }

    /// <summary>
    /// Uno shake leggerissimo per gli spari dell’arma giocosa.
    /// Pensato per frequenza 0.8 colpi/sec → deve essere rapido e subtle.
    /// </summary>
    public void PlayWrenchHit()
    {
        PlayShake(
            duration: 0.12f,            // molto breve
            posStrength: 0.03f,         // piccolissimo kick
            rotStrength: 2.5f,          // leggero recoil visivo
            vibratoPos: 8,
            vibratoRot: 10
        );
    }

    public void PlayJumpscare(float duration)
    {
        PlayShake(
            duration: duration,            // molto breve
            posStrength: 0.25f * .4f,
            rotStrength: 15f * .4f,
            vibratoPos: 30,
            vibratoRot: 20
        );
    }

    // -----------------------------
    // CORE SHAKE HANDLER
    // -----------------------------

    private void PlayShake(
        float duration,
        float posStrength,
        float rotStrength,
        int vibratoPos,
        int vibratoRot,
        System.Action onComplete = null, System.Action onKill = null)
    {
        // Ferma shake precedenti
        shakeTween?.Kill();
        rotTween?.Kill();

        // SHAKE POSITION
        shakeTween = transform.DOShakePosition(
            duration,
            strength: posStrength,
            vibrato: vibratoPos,
            randomness: 90,
            fadeOut: true
        ).SetUpdate(true);

        // SHAKE ROTATION
        rotTween = transform.DOShakeRotation(
            duration,
            strength: rotStrength,
            vibrato: vibratoRot,
            randomness: 90,
            fadeOut: true
        ).SetUpdate(true);

        rotTween.onComplete += () =>
        {
            transform.localPosition = originalPos;
            transform.localEulerAngles = originalRot;
            onComplete?.Invoke();
        };

        rotTween.onKill += () =>
        {
            transform.localPosition = originalPos;
            transform.localEulerAngles = originalRot;
            onKill?.Invoke();
        };


    }
}
