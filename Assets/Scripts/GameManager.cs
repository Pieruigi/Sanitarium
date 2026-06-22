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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = -1;// 60;
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
            SaveManager.Instance.Load();
        }

        StartCoroutine(DoLoad());

        IEnumerator DoLoad()
        {
            yield return null;
            SceneManager.LoadScene(GameSceneIndex);
        }

        //SceneManager.LoadScene(GameSceneIndex);


    }

    
}
