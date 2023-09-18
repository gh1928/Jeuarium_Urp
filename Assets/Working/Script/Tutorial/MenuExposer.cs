using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuExposer : MonoBehaviour
{
    public GameObject menu;
    bool isMenu = true;

    public void MenuExpose()
    {
        if (!isMenu)
        {
            menu.SetActive(true);
            isMenu = true;
        }
        else
        {
            menu.SetActive(false);
            isMenu = false;
        }
    }
}
