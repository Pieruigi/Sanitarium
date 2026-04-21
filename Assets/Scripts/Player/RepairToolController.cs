using UnityEngine;
using UnityEngine.Events;

namespace Baloon
{
    public class RepairToolController : Singleton<RepairToolController>
    {
       

        [SerializeField]
        GameObject wrench;

        bool equipped = false;
        public bool Equipped => equipped;

        [SerializeField]
        Animator animator;

        

        protected override void Awake()
        {
            base.Awake();

           
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            wrench.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ReportPickedUp()
        {
            equipped = true;
            wrench.SetActive(true);
        }

        public void ReportPutBack()
        {
            equipped = false;
            wrench.SetActive(false);
            if (animator.GetBool("Hit")) animator.SetBool("Hit", false);
        }

        public void StartRepairAnimation()
        {
            animator.SetBool("Hit", true);
        }

        public void StopRepairAnimation()
        {
            animator.SetBool("Hit", false);
        }

       
    }
}