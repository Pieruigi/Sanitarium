using UnityEngine;
using DG.Tweening;
using StarterAssets;
using JetBrains.Annotations;

public class CameraShake : Singleton<CameraShake>
{
    private Vector3 originalPos;
    private Vector3 originalRot;

    private Tween shakeTween;
    private Tween rotTween;

    private Tween scareShakeTween;
    private Tween scareRotTween;

    private Tween verticalWindShakeTween;
    private Tween verticalWindRotTween;

    [SerializeField]
    Transform scareTransform;

    [SerializeField]
    Transform verticalWindTransform;

    float windShakeMultiplier = 1f;//1.4f;

    protected override void Awake()
    {
        base.Awake();
        originalPos = transform.localPosition;
        originalRot = transform.localEulerAngles;
    }

#if UNITY_EDITOR
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    PlayLandingShake(0f);
        //}
    }
#endif

    public void PlayCatwalkCollapseShake(float duration, System.Action onComplete = null, System.Action onKill = null)
    {
        float randPos = Random.Range(0.006f * 20f, 0.009f * 20f);
        float randRot = Random.Range(0.02f * 20f, 0.04f * 20f);

        PlayShake(
            duration: duration,
            posStrength: randPos,
            rotStrength: randRot,
            vibratoPos: 16,
            vibratoRot: 16,
            onComplete,
            onKill,
            false
        );
    }

    public void PlayMoaningShake(float duration, System.Action onComplete = null, System.Action onKill = null)
    {
        float randPos = Random.Range(0.006f * 3f, 0.009f * 3f);
        float randRot = Random.Range(0.02f * 3f, 0.04f * 3f);

        PlayShake(
            duration: duration,
            posStrength: randPos,
            rotStrength: randRot,
            vibratoPos: 12,
            vibratoRot: 12,
            onComplete,
            onKill,
            fadeOut: true
        );
    }

    // -----------------------------
    // PUBLIC METHODS
    // -----------------------------
    public void PlayVerticalWindShake(float duration, System.Action onComplete = null, System.Action onKill = null)
    {
        float randDuration = duration;
        float randPos = Random.Range(0.006f*2f, 0.009f*2f) * windShakeMultiplier;
        float randRot = Random.Range(0.02f*2f, 0.04f*2f) * windShakeMultiplier;

        PlayShake(
            duration: randDuration,
            posStrength: randPos,
            rotStrength: randRot,
            vibratoPos: 0,
            vibratoRot: 0,
            onComplete, 
            onKill,
            jumpscare: false,
            verticalWind: true
        );

    }


    /// <summary>
    /// Stato VERDE: Altezza perfetta. 
    /// Un dondolio leggero quasi impercettibile, per dare vita all'ambiente.
    /// </summary>
    public void PlayWindShakeLight(System.Action onComplete = null, System.Action onKill = null)
    {
        float randDuration = Random.Range(3.2f, 4.0f);
        float randPos = Random.Range(0.012f, 0.018f) * windShakeMultiplier;
        float randRot = Random.Range(0.4f, 0.8f) * windShakeMultiplier;

        PlayShake(
            duration: randDuration,
            posStrength: randPos,
            rotStrength: randRot,
            vibratoPos: 1,
            vibratoRot: 1,
            onComplete, 
            onKill
        );
        
    }

 

    public void PlayWindShakeStrong(System.Action onComplete = null, System.Action onKill = null)
    {
        float randDuration = Random.Range(3.2f, 4.0f);
        float randPos = Random.Range(0.024f, 0.036f) * windShakeMultiplier;
        float randRot = Random.Range(0.8f, 1.6f) * windShakeMultiplier;
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

        float randPos = Random.Range(0.03f, 0.04f) * windShakeMultiplier;
        float randRot = Random.Range(1.5f, 2.5f) * windShakeMultiplier;
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
        float randPos = Random.Range(0.3f, 0.4f) * windShakeMultiplier;
        float randRot = Random.Range(2f, 2.85f) * windShakeMultiplier;
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
            vibratoRot: 20,
            jumpscare: true
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

    //public void PlayLanding()
    //{
    //    PlayShake(
    //        duration: 0.22f,            // molto breve
    //        posStrength: 0.06f,         // piccolissimo kick
    //        rotStrength: 4.5f,          // leggero recoil visivo
    //        vibratoPos: 10,
    //        vibratoRot: 13
    //    );
    //}

    public void PlayJumpscare(float duration)
    {
        PlayShake(
            duration: duration,            // molto breve
            posStrength: 0.25f * .4f,
            rotStrength: 15f * .4f,
            vibratoPos: 30,
            vibratoRot: 20,
            jumpscare: true
        );
    }

    public void PlayBlooderScream()
    {
        PlayShake(
            duration: 4f,            // molto breve
            posStrength: 0.25f * .1f,
            rotStrength: 15f * .1f,
            vibratoPos: 30,
            vibratoRot: 20,
            jumpscare: true
        );
    }

    public void PlayLandingShake(float force)
    {
        PlayShake(
           duration: Mathf.Lerp(0.4f, 0.5f, force),         // molto breve
           posStrength: Mathf.Lerp(.02f, 0.2f, force),         // piccolissimo kick
           rotStrength: Mathf.Lerp(.45f, 4.5f, force),          // leggero recoil visivo
           vibratoPos: (int)Mathf.Lerp(8, 16, force),
           vibratoRot: (int)Mathf.Lerp(13, 22, force)
       );
    }

    public void PlayTakeOffShake()
    {

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
        System.Action onComplete = null, System.Action onKill = null, bool fadeOut = true, bool jumpscare = false, bool verticalWind = false)
    {
        // Ferma shake precedenti

        Transform t = null;
        if (!jumpscare)
        {
            if (!verticalWind)
            {
                
                shakeTween?.Kill();
                shakeTween = null;
                rotTween?.Kill();
                rotTween = null;
                t = transform;
            }
            else
            {
                Debug.Log("TEST - WIND - Vertical is null:" + (verticalWindShakeTween == null));

                if (verticalWindShakeTween != null && verticalWindShakeTween.IsPlaying())
                {
                    verticalWindShakeTween.Kill();
                    verticalWindShakeTween = null;
                }

                if (verticalWindRotTween != null && verticalWindRotTween.IsPlaying())
                {
                    verticalWindRotTween.Kill();
                    verticalWindRotTween = null;
                }

                
                t = verticalWindTransform;
            }
            
        }
        else
        {
            scareShakeTween?.Kill();
            scareShakeTween = null;
            scareRotTween?.Kill();
            scareRotTween = null;   
            t = scareTransform;
        }

        Tween sTween, rTween;
        

        // SHAKE POSITION
        sTween = t.DOShakePosition(
            duration,
            strength: posStrength,
            vibrato: vibratoPos,
            randomness: 90,
            fadeOut: fadeOut
        ).SetUpdate(true);

        // SHAKE ROTATION
        rTween = t.DOShakeRotation(
            duration,
            strength: rotStrength,
            vibrato: vibratoRot,
            randomness: 90,
            fadeOut: fadeOut
        ).SetUpdate(true);

        sTween.onComplete += () =>
        {
            t.localPosition = originalPos;
            t.localEulerAngles = originalRot;

            if (sTween == verticalWindShakeTween)
                verticalWindShakeTween = null;
            else if (sTween == shakeTween)
                shakeTween = null;
            else
                scareShakeTween = null;

            onComplete?.Invoke();
        };

        sTween.onKill += () =>
        {
            if (sTween.IsComplete()) return;

            t.localPosition = originalPos;
            t.localEulerAngles = originalRot;

            if (sTween == verticalWindShakeTween)
                verticalWindShakeTween = null;
            else if (sTween == shakeTween)
                shakeTween = null;
            else
                scareShakeTween = null;

            onKill?.Invoke();
        };

        if (!jumpscare)
        {
            if (!verticalWind)
            {
                shakeTween = sTween;
                rotTween = rTween;
            }
            else
            {
                verticalWindShakeTween = sTween;
                verticalWindRotTween = rTween;
            }
        }
        else
        {
            scareShakeTween = sTween;
            scareRotTween = rTween;
        }

    }

   
}
