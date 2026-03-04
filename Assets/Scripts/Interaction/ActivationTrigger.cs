using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

namespace TMM
{
	public class ActivationTrigger : MonoBehaviour
	{
		public delegate void EnterDelegate(Collider other);
		public delegate void ExitDelegate(Collider other);
		public EnterDelegate OnEnter;
		public ExitDelegate OnExit;

		bool disabled = false;
		
	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

		void OnTriggerEnter(Collider other)
		{
			if (disabled) return;
			if (other.CompareTag("Player"))
			{
                Debug.Log("OnEnter");
                OnEnter?.Invoke(other);
            }
				
		}

		void OnTriggerExit(Collider other)
		{
            if(disabled) return;
            if (other.CompareTag("Player"))
			{
                Debug.Log("OnExit");
                OnExit?.Invoke(other);
            }
				
		}
		
		public void SetEnabled(bool value)
        {
			disabled = !value;
			
        }

		public bool IsEnabled()
		{
            return !disabled;
        }
    }
}
