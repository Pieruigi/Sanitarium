using UnityEngine;

namespace Baloon
{


    public class PlayerMap : MonoBehaviour
    {
        [SerializeField]
        GameObject root;

        [SerializeField]
        float distance = .5f;

        bool open = false;

        private void Awake()
        {
            root.SetActive(false);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!open)
                    Open();
                else
                    Close();
            }
            
        }

        private void LateUpdate()
        {
            if (!open) return;

            //// Get camera position and forward
            //var camPos = Camera.main.transform.position;
            //var camFwd = Camera.main.transform.forward;

            //// Adjust this position and rotation
            //var pos = camPos + camFwd * distance;
            //transform.position = pos;
            //transform.forward = camFwd;
        }

        void Open()
        {
            if (open) return;
            open = true;
            root.SetActive(open);
        }

        void Close()
        {
            if (!open) return;
            open = false;
            root.SetActive(open);
        }
    }
}