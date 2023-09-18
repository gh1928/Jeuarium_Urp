using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] bool Basic_isEnd;

    BNG.InputBridge input;
    bool ANM_isDebug = false;

    private void Start()
    {
        input = BNG.InputBridge.Instance;

#if UNITY_EDITOR
        ANM_isDebug = false;
#endif
    }

    private void Update()
    {
        if (ANM_isDebug)
        {
            if (input.BButton || Keyboard.current.rKey.isPressed)
            {
                ANM_LoadNextScene();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Basic_isEnd)
            {
                LoadMainScene();
            }
            else
            {
                ANM_LoadNextScene();
            }
        }
    }

    public void ANM_LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadOtherScene(string s)
    {
        SceneManager.LoadScene(s);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
