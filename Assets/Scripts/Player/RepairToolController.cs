using UnityEngine;

namespace Baloon
{
    public class RepairToolController : Singleton<RepairToolController>
    {
        [SerializeField]
        GameObject wrench;

        bool equipped = false;
        public bool Equipped => equipped;

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
        }
    }
}