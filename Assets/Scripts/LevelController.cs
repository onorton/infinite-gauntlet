using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single); // loads current scene
    }

    public void NextLevel(string level)
    {
        SceneManager.LoadScene(level, LoadSceneMode.Single); // loads specified scene
    }
}
