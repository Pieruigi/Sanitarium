using Baloon;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarrionFlyPatroller : MonoBehaviour
{
    [SerializeField]
    List<Transform> patrolPoints;

    [SerializeField]
    float idleTimeMin = 3;

    [SerializeField]
    float idleTimeMax = 5f;

    [SerializeField]
    Animator animator;

    bool moving = false;

    float time;

    Transform currentPoint;

    bool attacking = false;

    GameObject attackPoint;

    float moveSpeed = 10f;
    

    private void Awake()
    {
        
        // Set starting position
        currentPoint = patrolPoints[Random.Range(0, patrolPoints.Count)];
        transform.position = currentPoint.position;
        transform.rotation = currentPoint.rotation;

        // Init idle time
        time = Random.Range(idleTimeMin, idleTimeMax);

        // Set idle animation
        SetIdleAnimation();

        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.X))
            StartAttacking();
#endif
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

        currentPoint = patrolPoints[Random.Range(0, patrolPoints.Count)];
        // Init idle time
        time = Random.Range(idleTimeMin, idleTimeMax);
        // Move
        SetMoveAnimation();
    }

    public void StartAttacking()
    {
        attacking = true;
        moving = true;

        var balloon = BaloonController.Instance;

        // Choose the boiler side
        var dir = Vector3.ProjectOnPlane(balloon.transform.position - transform.position, Vector3.up);
        var dot = Vector3.Dot(balloon.transform.forward, dir);

        // Create and attach the attack point to the balloon
        attackPoint = new GameObject($"{gameObject.name}_AttackPoint");
        attackPoint.transform.parent = balloon.transform;
        
        if(dot > 0) // Back attack
        {
            attackPoint.transform.localPosition = -1f * balloon.transform.forward;
            attackPoint.transform.localEulerAngles = Vector3.zero;
        }
        else // Front attack
        {
            attackPoint.transform.localPosition = 1f * balloon.transform.forward;
            attackPoint.transform.localEulerAngles = Vector3.up * 180f;
        }

        // Set height
        attackPoint.transform.localPosition += Vector3.up * 2.5f;
        attackPoint.transform.localPosition += balloon.transform.right * Random.Range(-.3f, .3f);

        // Move
        SetMoveAnimation();

    }


    public void Hit()
    {
        if (!attackPoint) return;

        BaloonBoilerHealth.Instance.TryTakeSingleDamage();

        StartCoroutine(StopAttackingDelayed(.5f));

        IEnumerator StopAttackingDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            StopAttacking();
        }
    }
    
}
