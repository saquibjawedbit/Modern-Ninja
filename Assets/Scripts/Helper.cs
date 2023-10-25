using UnityEngine;
using UnityEngine.SceneManagement;

public class Helper : MonoBehaviour
{

    public GameObject loadingObject;
    [SerializeField] private int nextScene = 0;
    [SerializeField] private int currentScene = 0;
    public void PauseTheGame()
    {
        Time.timeScale = 0;
    }

    public void Restart()
    {
        GetBackToMenu(currentScene);
    }

    public void NextScene()
    {
        GetBackToMenu(nextScene);
    }

    public void ResumeTheGame()
    {
        Time.timeScale = 1;
    }

    public void GetBackToMenu(int index)
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(index);

        loadingObject.SetActive(true);

    }
}
