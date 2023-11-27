using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option : MonoBehaviour
{
    public GameObject option;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public void ToggleOption()
    {
        option.SetActive(option.activeSelf);
    }
    public void Update()
    {
        if(BNG.InputBridge.Instance.YButton)
        {
            ToggleOption();
        }
    }
    public void OnChangeAudioVolume(float volume)
    {

    }
}
