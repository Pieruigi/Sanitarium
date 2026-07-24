using Baloon;
using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CarrionFlyController : MonoBehaviour
{
    [SerializeField]
    List<Transform> patrolPoints;

    [SerializeField]
    float idleTimeMin = 3;

    [SerializeField]
    float idleTimeMax = 5f;

    [SerializeField]
    Animator animator;

    [SerializeField]
    ParticleSystem fireParticlePrefab;

    [SerializeField]
    Renderer meshRenderer;

    [SerializeField]
    AudioSource buzzAudioSource;

    [SerializeField]
    AudioSource hitAudioSource;

    [SerializeField]
    AudioSource burningAudioSource;

    [SerializeField]
    AudioSource screechAudioSource;

    bool moving = false;

    float time;

    Transform currentPoint;

    bool attacking = false;

    bool attackOnStart = false;

    GameObject attackPoint;

    float moveSpeed = 10f;

    BaloonBoilerHealthVfx leakManager;

    Rigidbody rb;

    bool isDead = false;
    public bool IsDead {  get { return isDead; } }

    

    private void Awake()
    {
        
        // Get rigidbosy
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // Set starting position
        if(patrolPoints != null && patrolPoints.Count > 0)
        {
            currentPoint = patrolPoints[Random.Range(0, patrolPoints.Count)];
            transform.position = currentPoint.position;
            transform.rotation = currentPoint.rotation;
        }
        else
        {
            // If we don't have any patrol point then we are spawning the carrion fly only to attack the balloon
            attackOnStart = true; 
        }

            // Init idle time
            time = Random.Range(idleTimeMin, idleTimeMax);

        // Set idle animation
        SetIdleAnimation();

        // Ignore balloon hit collision
        var coll = GetComponent<Collider>();
        var others = FindObjectsByType<BalloonCollisionChecker>(FindObjectsSortMode.None);
        foreach (var other in others)
            Physics.IgnoreCollision(coll, other.GetComponent<Collider>(), true);
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leakManager = BaloonController.Instance.GetComponentInChildren<BaloonBoilerHealthVfx>();

        if (attackOnStart)
        {
            attackOnStart = false;
            StartAttacking();
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead) return;

        if (!attacking)
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                var list = patrolPoints;//.Where(p => p != currentPoint).ToList();
                
                currentPoint = list[Random.Range(0, list.Count)];

                // Change animation
                moving = true;
                SetMoveAnimation();

                time = Random.Range(idleTimeMin, idleTimeMax);

            }

            transform.position = Vector3.Lerp(transform.position, currentPoint.position, moveSpeed * Time.deltaTime);

            var rot = currentPoint.rotation;
            if (moving)
            {
                var dir = Vector3.ProjectOnPlane(currentPoint.position - transform.position, Vector3.up);
                rot = Quaternion.LookRotation(dir, Vector3.up);
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, rot, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentPoint.position) < 1f)
            {
                if (moving)
                {
                    moving = false;
                    SetIdleAnimation();
                }

            }
        }
        else // Is attacking
        {
            

            transform.position = Vector3.Lerp(transform.position, attackPoint.transform.position, moveSpeed * Time.deltaTime);

            var rot = attackPoint.transform.rotation;
            if (moving)
                rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(attackPoint.transform.position - transform.position, Vector3.up), Vector3.up);

            transform.rotation = Quaternion.Lerp(transform.rotation, rot, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, attackPoint.transform.position) < 1f)
            {
                if (moving)
                {
                    moving = false;
                    SetIdleAnimation();
                    StartCoroutine(AttackDelayed(.5f));
                }

            }
        }

#if UNITY_EDITOR
        //if (Input.GetKeyDown(KeyCode.X))
        //    StartAttacking();
#endif
    }

    private void FixedUpdate()
    {
        if (!isDead) return;

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        var balloonCollider = BaloonController.Instance.GetComponent<Collider>();
        if(collision.collider == balloonCollider)
        {
            // reset velocity
            rb.linearVelocity = Vector3.zero;
            // Set kinematic
            rb.isKinematic = true;
            // Set parenting
            transform.parent = BaloonController.Instance.transform;
        }
    }

    void SetIdleAnimation()
    {
        if(animator.GetBool("Flying")) animator.SetBool("Flying", false);
    }

    void SetMoveAnimation()
    {
        if (!animator.GetBool("Flying")) animator.SetBool("Flying", true);
    }

   

    IEnumerator AttackDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        animator.SetTrigger("Attack");

    }

    public void StopAttacking()
    {
        attacking = false;
        moving = true;

        // Destry the attack point
        Destroy(attackPoint);

        currentPoint = patrolPoints[Random.Range(0, patrolPoints.Count)];
        // Init idle time
        time = Random.Range(idleTimeMin, idleTimeMax);
        // Move
        SetMoveAnimation();
    }

    public void StartAttacking()
    {


        var balloon = BaloonController.Instance;
        var player = GameObject.FindGameObjectWithTag("Player");

        // Choose the boiler side
        var dir = Vector3.ProjectOnPlane(balloon.transform.position - transform.position, Vector3.up);
        var dirP = Vector3.ProjectOnPlane(balloon.transform.position - player.transform.position, Vector3.up);
        var dot = Vector3.Dot(dirP.normalized, dir.normalized);
        


        BaloonBoilerLeak leak = null;
        List<BaloonBoilerLeak> leaks = null;


        if (dot > 0) // Player is looking in the panel control direction (-0.342f for 110 degrees, -0.174f for 100 degrees)
            leaks = leakManager.Leaks.Where(l => !l.Damaged && Vector3.Dot(dir.normalized, Vector3.ProjectOnPlane(l.transform.forward, Vector3.up).normalized) > 0.75f).ToList();

        
        if(leaks == null || leaks.Count == 0) // May happens it dots is <0 or previous conditions are not satisfied
            leaks = leakManager.Leaks.Where(l => !l.Damaged /*&& Vector3.Dot(dir, Vector3.ProjectOnPlane(l.transform.forward, Vector3.up)) > 0*/).ToList();


        if (leaks == null || leaks.Count == 0) return; // Stop

        leak = leaks[Random.Range(0, leaks.Count)];

        leakManager.NextToHit = leak;

        // Create and attach the attack point to the balloon
        attackPoint = new GameObject($"{gameObject.name}_AttackPoint");
        attackPoint.transform.parent = leak.transform;

        var pos = leak.transform.position;
        var fwd = Vector3.ProjectOnPlane(leak.transform.forward, Vector3.up);

        attackPoint.transform.position = pos + fwd * 1f + Vector3.down * .15f;// - fwd * .8f;
        attackPoint.transform.forward = -fwd;

        // Set height
        //attackPoint.transform.localPosition += Vector3.up * 2.5f;
        //attackPoint.transform.localPosition += balloon.transform.right * Random.Range(-.3f, .3f);

        // We set flags once we are sure the attack can start
        attacking = true;
        moving = true;

        // Move
        SetMoveAnimation();

    }

    public void _StartAttacking()
    {
        

        var balloon = BaloonController.Instance;

        // Choose the boiler side
        var dir = Vector3.ProjectOnPlane(balloon.transform.position - transform.position, Vector3.up);
        var dot = Vector3.Dot(balloon.transform.forward, dir);

        

        BaloonBoilerLeak leak = null;
        List<BaloonBoilerLeak> leaks = null;


        if (dot > 0) 
            leaks = leakManager.Leaks.Where(l => !l.Damaged && Vector3.Dot(dir, Vector3.ProjectOnPlane(l.transform.forward, Vector3.up)) < 0).ToList();
        else 
            leaks = leakManager.Leaks.Where(l => !l.Damaged && Vector3.Dot(dir, Vector3.ProjectOnPlane(l.transform.forward, Vector3.up)) > 0).ToList();


        if (leaks == null || leaks.Count == 0) return; // Stop

        leak = leaks[Random.Range(0, leaks.Count)];

        leakManager.NextToHit = leak;

        // Create and attach the attack point to the balloon
        attackPoint = new GameObject($"{gameObject.name}_AttackPoint");
        attackPoint.transform.parent = leak.transform;

        var pos = leak.transform.position;
        var fwd = Vector3.ProjectOnPlane(leak.transform.forward, Vector3.up);

        attackPoint.transform.position = pos + fwd * 1.1f + Vector3.down * .12f;// - fwd * .8f;
        attackPoint.transform.forward = -fwd;
        
        // Set height
        //attackPoint.transform.localPosition += Vector3.up * 2.5f;
        //attackPoint.transform.localPosition += balloon.transform.right * Random.Range(-.3f, .3f);

        // We set flags once we are sure the attack can start
        attacking = true;
        moving = true;

        // Move
        SetMoveAnimation();

    }


    public void Hit()
    {
        if (!attackPoint) return;

        hitAudioSource.Play();

        BaloonBoilerHealth.Instance.TryTakeSingleDamage();

        StartCoroutine(Die());
        
        IEnumerator Die()
        {
            yield return new WaitForSeconds(.25f);

            

            // Stop buzz audio source
            buzzAudioSource.Stop();

            // Burn
            burningAudioSource.Play();

            // Screech
            screechAudioSource.PlayDelayed(.7f);

            // Create particle
            var fire = Instantiate(fireParticlePrefab);
            fire.transform.parent = transform;
            fire.transform.position = transform.position + Vector3.up * .365f + transform.forward * .091f; ;
            fire.transform.rotation = Quaternion.identity;

            Destroy(fire.gameObject, 5f);

            Darken(meshRenderer, 1f, Color.white * .4f);
          
            yield return new WaitForSeconds(1f);

            rb.isKinematic = false;
            rb.useGravity = true;

            var vel = BaloonController.Instance.CurrentVelocity * 1.2f;
            vel.y = 0;

            rb.linearVelocity = vel;

            // Start dying animation
            animator.SetTrigger("Die");

            isDead = true;
        }


    }

    void Darken(Renderer targetRenderer, float duration, Color targetColor)
    {
        // English: Accessing .materials creates unique instances for this specific renderer,
        // ensuring other objects using the same shared material are not affected.
        Material[] materials = targetRenderer.materials;

        foreach (Material mat in materials)
        {
            // Avvia il tween sul colore del materiale
            mat.DOColor(targetColor, "_BaseColor", duration);
        }
    }

}
