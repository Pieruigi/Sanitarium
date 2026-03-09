using System;
using UnityEngine;


namespace SNT
{
    public class CameraOnOff : MonoBehaviour
    {
        [SerializeField]
        GameObject target;


        bool isOff = false;

    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
      
        private void SwitchOnOff()
        {
            
            
            isOff = !isOff;
            if(isOff)
            {
                target.GetComponent<Renderer>().material.color = Color.black;
            }
            else
            {
                target.GetComponent<Renderer>().material.color = Color.white;
            }

        }
    }
}