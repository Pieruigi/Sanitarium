using Baloon;
using Baloon.SaveSystem;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonPersistent<GameManager>
{
 
    public const int MainSceneIndex = 0;
    public const int GameSceneIndex = 1;


    bool isNewGame = false;
    public bool IsNewGame => isNewGame;

    //public int Difficulty = 0;

    public bool ShowStory = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = -1;// 60;

        //Difficulty = SettingsManager.Instance.GameMode;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        
        isNewGame = !SaveManager.Instance.SaveFileExists();
        if (!isNewGame)
        {
            ShowStory = false;
            SaveManager.Instance.Load();
        }
        else
        {
            ShowStory = true;
        }

        StartCoroutine(DoLoad());

        IEnumerator DoLoad()
        {
            yield return null;
            SceneManager.LoadScene(GameSceneIndex);
        }

        //SceneManager.LoadScene(GameSceneIndex);


    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene(MainSceneIndex);
    }

    public void ReportPlayerDeath()
    {
        if(SettingsManager.Instance.GameMode == 0)
        {
            // Delete save game
            SaveManager.Instance.Delete();
        }

        //PlayGame();
        LoadMainScene();
    }
    
}
