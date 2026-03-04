using UnityEngine;

public class TestPlatform : MonoBehaviour
{
    bool flying = false;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.X))
        //{
        //    flying = !flying;
        //}

        
    }

    private void FixedUpdate()
    {
        if(flying)
        {
            rb.AddForce(Vector3.up * 20, ForceMode.Acceleration);
            
        }
    }

}
