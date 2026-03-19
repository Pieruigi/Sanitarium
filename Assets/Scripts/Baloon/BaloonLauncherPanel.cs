using DG.Tweening;
using System;
using TMM;
using UnityEngine;

public class BaloonLauncherPanel : MonoBehaviour
{
    [SerializeField]
    ActivationTrigger activator;

    [SerializeField]
    Transform root;

    bool activated = false;
    public bool Activated => activated;

    float ySpeed = 25f;

    GameObject player;

    float yDefault;

    bool inside = false;

    float currentOffset = 0;

    private void Awake()
    {
        yDefault = root.position.y;
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
        if(inside && !activated)
        {
            activated = true;
            root.DOKill();
            currentOffset = player.transform.position.y - yDefault;
        }
        else if(!inside && activated)
        {
            activated = false;
            root.DOKill();

            root.DOMoveY(yDefault, .5f).SetEase(Ease.InBounce);
        }

        if (activated)
        {
            var rootPos = root.position;
            var yTarget = player.transform.position.y;// - currentOffset;

            rootPos.y = Mathf.Lerp(rootPos.y, yTarget, ySpeed * Time.deltaTime);

            root.position = rootPos;
        }



        

    }

    private void OnEnable()
    {
        activator.OnEnter += HandleOnEnter;
        activator.OnExit += HandleOnExit;
    }

    private void OnDisable()
    {
        activator.OnEnter -= HandleOnEnter;
        activator.OnExit -= HandleOnExit;
    }

    private void HandleOnEnter(Collider other)
    {
        inside = true;
    }

    private void HandleOnExit(Collider other)
    {
        inside = false;
    }

   

  
}
