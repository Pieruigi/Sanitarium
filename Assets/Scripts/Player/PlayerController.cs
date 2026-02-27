using SNT.UI;
using UnityEngine;

namespace SNT
{
    public enum PlayerState { None, Normal }

    public class PlayerController : Singleton<PlayerController>
    {
        PlayerState state = PlayerState.None;

        float interactDistance = 3f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SetState(PlayerState.Normal);
        }

        // Update is called once per frame
        void Update()
        {
           
            UpdateState();

        }

        public void ShowCursor(bool value)
        {
            Cursor.visible = value;
            //Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        }

        void SetState(PlayerState newState)
        {
            if (state == newState) return;

            PlayerState oldState = state;

            state = newState;

            switch (state)
            {
                case PlayerState.Normal:
                    EnterNormalState();
                    break;
            }
        }

        void UpdateState()
        {
            switch (state)
            {
                case PlayerState.Normal:
                    UpdateNormalState();
                    break;
            }
        }

        void EnterNormalState()
        {
            PlayerDot.Instance.ShowDot(true);
            ShowCursor(false);
        }

        void UpdateNormalState()
        {
            // Check input
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, interactDistance, LayerMask.GetMask(new string[] { "Interactable" })))
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);
            }
        }

    }

}
