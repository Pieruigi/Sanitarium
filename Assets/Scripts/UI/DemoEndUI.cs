using Baloon.UI;
using UnityEngine;

namespace Baloon.Demo
{
    public class DemoEndUI : Singleton<DemoEndUI>
    {
        [SerializeField]
        GameObject panel;

        protected override void Awake()
        {
            base.Awake();

#if !DEMO
            Destroy(gameObject);
#else
            panel.SetActive(false);
#endif

        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Show()
        {
            panel.SetActive(true);
            MenuUI.Instance.Unavailable = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void OpenSteam()
        {
            Application.OpenURL("https://store.steampowered.com/app/4839050/Altitude_Zero/");
            GameManager.Instance.LoadMainScene();
        }
    }

}
