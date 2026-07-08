using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Baloon.UI
{
    public class QuitButton : MonoBehaviour
    {
        private void Awake()
        {
            if (SceneManager.GetActiveScene().buildIndex == GameManager.MainSceneIndex)
                GetComponent<Button>().onClick.AddListener(() => { Application.Quit(); });
            else
                GetComponent<Button>().onClick.AddListener(() => { GameManager.Instance.LoadMainScene(); });
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}