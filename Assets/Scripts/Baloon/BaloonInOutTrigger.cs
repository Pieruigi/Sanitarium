using UnityEngine;

namespace Baloon
{
    public class BaloonInOutTrigger : MonoBehaviour
    {
        [SerializeField]
        RepairToolPicker repairToolPicker;

        RepairToolController repairTool;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            repairTool = FindFirstObjectByType<RepairToolController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (repairTool.Equipped) 
            {
                // Put back the tool
                repairToolPicker.PutBack();
            }
        }
    }
}