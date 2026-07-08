using Baloon.SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Baloon.UI
{
    public class MenuUI : Singleton<MenuUI>
    {
        [SerializeField]
        GameObject gameMode;

        [SerializeField]
        GameObject logo;

        [SerializeField]
        GameObject panel;

        [SerializeField]
        Button clearButton;

        bool inGame = false;

        bool unavailable = false;
        public bool Unavailable
        {
            get { return unavailable; }
            set 
            {
                if (value && panel.activeSelf)
                {
                    panel.SetActive(false);
                }
                unavailable = value;
            }
        }
        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if(SceneManager.GetActiveScene().buildIndex == GameManager.GameSceneIndex)
            {
                gameMode.SetActive(false);
                logo.SetActive(false);
                panel.SetActive(false); 
                inGame = true;
        
            }
            else
            {
                if (!SaveManager.Instance.SaveFileExists())
                    clearButton.interactable = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!inGame || unavailable) return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (panel.activeSelf)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
                    
            }
        }

        public void Hide()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            panel.SetActive(false);
            Time.timeScale = 1;
        }

        void Show()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            panel.SetActive(true);
            Time.timeScale = 0;
        }
    }
}