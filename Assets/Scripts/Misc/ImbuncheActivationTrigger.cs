using UnityEngine;

namespace Baloon
{
    public class ImbuncheActivationTrigger : MonoBehaviour
    {
        [SerializeField]
        GameObject target;

        [SerializeField]
        int startPatrolIndex = 0;

        [SerializeField]
        Transform startPoint;

        [SerializeField]
        bool deactivate = false;
     
        private void Awake()
        {
            
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (!deactivate && !target.activeSelf)
            {
                target.transform.position = startPoint.position;
                target.transform.rotation = startPoint.rotation;
                target.GetComponent<ImbuncheController>().SetPatroPoint(startPatrolIndex);
                target.SetActive(true);
            }
            else
            {
                if(deactivate)
                    target.SetActive(false);
            }

            

        }

        
    }
}