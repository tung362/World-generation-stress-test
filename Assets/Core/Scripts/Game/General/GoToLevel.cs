using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToLevel : MonoBehaviour
{
    public string LevelName;

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(LevelName);
    }
}
