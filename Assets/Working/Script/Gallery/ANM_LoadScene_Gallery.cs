using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using UnityEngine.SceneManagement;

public class ANM_LoadScene_Gallery : LoadScene
{
    //////////  Getter & Setter //////////
    
    //////////  Method          //////////

    public void ANM_LoadStudio()
    {
        SceneManager.LoadScene("Studio");
    }

    public void ANM_LoadGogh()
    {
        SceneManager.LoadScene("Gogh_001.GoghRoom");
    }

    public void ANM_LoadMonet()
    {
        SceneManager.LoadScene("Monet_01.¹öÀü2");
        //SceneManager.LoadScene("Monet_01.¹Ù´å°¡Ç³°æ");
    }

    //////////  Unity           //////////
}
