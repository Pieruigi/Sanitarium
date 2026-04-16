using NUnit.Framework;
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

    bool moving = false;

    float time;

    Transform currentPoint;

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
        time -= Time.deltaTime;
        if(time < 0)
        {
            var list = patrolPoints.Where(p => p != currentPoint).ToList();
            currentPoint = list[Random.Range(0, list.Count)];

            // Change animation
            moving = true;
            SetMoveAnimation();

            time = Random.Range(idleTimeMin, idleTimeMax);
            
        }

        transform.position = Vector3.Lerp(transform.position, currentPoint.position, 10f * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, currentPoint.rotation, 10f * Time.deltaTime);

        if(Vector3.Distance(transform.position, currentPoint.position) < .1f)
        {
            if(moving)
            {
                moving = false;
                SetIdleAnimation();
            }
        }      

    }

    void SetIdleAnimation()
    {

    }

    void SetMoveAnimation()
    {

    }
}
