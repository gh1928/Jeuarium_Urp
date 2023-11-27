using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void StartScene()
    {
        Debug.Log("start scene button clicked");
        SceneManager.LoadScene(1);
    }

    public void LoadScene()
    {

    }

    public void Quit()
    {
        Debug.Log("quit button clicked");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
