using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteMenu : MonoBehaviour
{
    public GameObject main;
    public GameObject paint;
    public GameObject option;
    bool isPaint = false;

    private void Start()
    {
        main.SetActive(true);
        paint.SetActive(false);
        option.SetActive(false);
        isPaint = false;
    }

    public void ChangeMenu()
    {
        if (!isPaint)
        {
            main.SetActive(false);
            paint.SetActive(true);
            option.SetActive(true);
            isPaint = true;
        }
        else
        {
            main.SetActive(true);
            paint.SetActive(false);
            option.SetActive(false);
            isPaint = false;
        }
    }
}
