using UnityEngine;

namespace Baloon.UI
{
    public class StoryUI : MonoBehaviour
    {

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (GameManager.Instance.ShowStory)
            {
                MenuUI.Instance.Unavailable = true;

                GameManager.Instance.ShowStory = false;
                Time.timeScale = 0;

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Continue()
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            gameObject.SetActive(false);
            MenuUI.Instance.Unavailable = false;
        }
    }
}