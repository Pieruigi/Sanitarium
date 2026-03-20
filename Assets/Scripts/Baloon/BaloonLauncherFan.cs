using Baloon;
using DG.Tweening;
using System;
using TMM;
using UnityEngine;


public class BaloonLauncherFan : MonoBehaviour
{
    [SerializeField]
    ActivationTrigger activator;

    [SerializeField]
    Transform root;

    [SerializeField]
    Transform pivot;

    bool inside = false;

    float followSpeed = 10;

    GameObject player;

    BaloonLauncher baloonLauncher;

    private void Awake()
    {
        baloonLauncher = GetComponentInParent<BaloonLauncher>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (!inside) return;

        var range = AltitudeManager.Instance.GetCurrentRange();
        switch (range)
        {
            case AltitudeRange.Green:
                var pos = root.position;
                pos.y = Mathf.Lerp(pos.y, player.transform.position.y, followSpeed * Time.deltaTime);
                root.position = pos;
                break;
        }
    }

    private void OnEnable()
    {
        activator.OnEnter += HandleOnEnter;
        activator.OnExit += HandleOnExit;
        BaloonLauncher.OnDirectionChanged += HandleOnDirectionChanged;
        BaloonPathManager.OnPathSet += HandleOnLaunched;
    }

    private void OnDisable()
    {
        activator.OnEnter -= HandleOnEnter;
        activator.OnExit -= HandleOnExit;
        BaloonLauncher.OnDirectionChanged -= HandleOnDirectionChanged;
        BaloonPathManager.OnPathSet -= HandleOnLaunched;
    }

    private void HandleOnLaunched()
    {
        if(!inside) return;
        //throw new NotImplementedException();
    }

    private void HandleOnDirectionChanged(BaloonLauncher baloonLauncher)
    {
        if (this.baloonLauncher != baloonLauncher) return;

        // Get the current direction
        RotateToDirection(baloonLauncher.CurrentDirection);
        
        

    }

    private void HandleOnEnter(Collider other)
    {
        inside = true;
    }

    private void HandleOnExit(Collider other)
    {
        inside = false;
    }

    void RotateToDirection(int direction)
    {
        var currentY = pivot.eulerAngles.y;
        float targetY = 0;

        targetY = direction * 90f;

        // Keep clockwise
        while (targetY <= currentY)
            targetY += 360f;

        var duration = Mathf.Abs(currentY - targetY) / 90f;
        //duration *= .5f;

        
        pivot.DOKill();
        pivot.transform.DORotate(new Vector3(0, targetY, 0), duration, RotateMode.FastBeyond360).SetEase(Ease.OutBack, 1.2f);
    }
}
