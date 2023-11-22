using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ANM_Gallery_Manager : MonoBehaviour
{

    ////////// Getter & Setter  //////////

    ////////// Method           //////////

    ////////// Unity            //////////
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ANM_UI_Update();
    }
}

partial class ANM_Gallery_Manager
{
    [Header("UI ==================================================")]
    [SerializeField] GameObject         UI_mainMenu;
    [SerializeField] Transform          UI_optionTarget;
    [SerializeField] TitleController    UI_TitleController;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public void ANM_UI_BtnStart()
    {
        UI_mainMenu.SetActive(false);
        UI_TitleController.enabled = true;
    }

    public void ANM_UI_BtnLoad()
    {
        this.GetComponent<ANM_LoadScene_Gallery>().ANM_LoadSaveScene();
    }

    public void ANM_UI_BtnOption()
    {
        UI_mainMenu.SetActive(false);
        ANM_Option_Manager.ANM_Static_instance.ANM_ETC_Open(UI_mainMenu, UI_optionTarget);
    }

    public void ANM_UI_BtnExit()
    {
        Application.Quit();
    }

    ////////// Unity            //////////
    void ANM_UI_Update()
    {
        if(UI_mainMenu.activeSelf)
        {

        }
    }
}