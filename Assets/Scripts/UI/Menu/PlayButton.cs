using Baloon.SaveSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Baloon.UI
{
    public class PlayButton : MonoBehaviour
    {
        bool isGameScene;
        bool isGameSaved;

        LocalizeStringEvent locString;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            locString = GetComponentInChildren<LocalizeStringEvent>();

            isGameScene = SceneManager.GetActiveScene().buildIndex == GameManager.GameSceneIndex;
            if (!isGameScene)
                isGameSaved = SaveManager.Instance.SaveFileExists();

            if (isGameScene)
                InitCloseGameMenuButton();
            else if (isGameSaved)
                InitContinueGameButton();
            else
                InitNewGameButton();

        }

        // Update is called once per frame
        void Update()
        {

        }

        void InitCloseGameMenuButton()
        {
            locString.StringReference.TableEntryReference = "continue_game";
            GetComponent<Button>().onClick.AddListener(MenuUI.Instance.Hide);
        }

        void InitContinueGameButton()
        {
            locString.StringReference.TableEntryReference = "continue_game";
            GetComponent<Button>().onClick.AddListener(GameManager.Instance.PlayGame);
        }

        void InitNewGameButton()
        {
            locString.StringReference.TableEntryReference = "new_game";
            GetComponent<Button>().onClick.AddListener(GameManager.Instance.PlayGame);
        }

        public void ResetToNewGameButton()
        {
            locString.StringReference.TableEntryReference = "new_game";
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(GameManager.Instance.PlayGame);
        }
    }
}