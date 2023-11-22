using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using UnityEngine.UI;

public partial class ANM_Option_Manager : MonoBehaviour
{
    ////////// Getter & Setter  //////////

    ////////// Method           //////////

    ////////// Unity            //////////
    // Start is called before the first frame update
    void Start()
    {
        ANM_Static_Start();
    }

    // Update is called once per frame
    void Update()
    {
        ANM_ETC_Update();
    }
}

partial class ANM_Option_Manager
{
    static ANM_Option_Manager Static_instance = null;

    ////////// Getter & Setter  //////////
    public static ANM_Option_Manager ANM_Static_instance { get { return Static_instance;    }   }

    ////////// Method           //////////

    ////////// Unity            //////////
    void ANM_Static_Start()
    {
        if (Static_instance == null)
        {
            Static_instance = this;

            //
            DontDestroyOnLoad(this.gameObject);

            ANM_Graphic_Start();
            ANM_Sound_Start();
        }
        else
        {
            Destroy(this);
        }
    }
}

partial class ANM_Option_Manager
{
    public enum GRAPHIC_TYPE
    {
        LOW,
        NORMAL,
        HIGH
    }

    [Header("SOUND ==================================================")]
    [SerializeField] TMPro.TextMeshProUGUI Graphic_text;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public void ANM_Graphic_Select(int _option)
    {
        int graphic = PlayerPrefs.GetInt("OPTION_GRAPHIC", 0) + _option;
        if (graphic < 0)        { graphic = 0;  }
        else if (graphic > 1)   { graphic = 1;  }

        PlayerPrefs.SetInt("OPTION_GRAPHIC", _option);

        ANM_Graphic_Setting();
    }

    void ANM_Graphic_Setting()
    {
        int graphic = PlayerPrefs.GetInt("OPTION_GRAPHIC", 0);

        string text = "";
        switch(graphic)
        {
            case 0: { text = "∫∏≈Î";  }   break;
            case 1: { text = "≥∑¿Ω";  }   break;
        }
        Graphic_text.text = text;
    }

    ////////// Unity            //////////
    void ANM_Graphic_Start()
    {
        ANM_Graphic_Setting();
    }
}

partial class ANM_Option_Manager
{
    [Header("SOUND ==================================================")]
    [SerializeField] TMPro.TextMeshProUGUI Sound_text;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public void ANM_Sound_Select(int _option)
    {
        int sound = PlayerPrefs.GetInt("OPTION_SOUND", 5) + _option;
        if (sound < 0)          { sound = 0;    }
        else if (sound > 10)    { sound = 10;   }

        PlayerPrefs.SetInt("OPTION_SOUND", sound);

        //
        ANM_Sound_Setting();
    }

    void ANM_Sound_Setting()
    {
        int sound = PlayerPrefs.GetInt("OPTION_SOUND", 5);
        Sound_text.text = sound.ToString();
    }

    ////////// Unity            //////////
    void ANM_Sound_Start()
    {
        ANM_Sound_Setting();
    }
}

partial class ANM_Option_Manager
{
    [Header("RUNNING")]
    [Header("ETC ==================================================")]
    [SerializeField] GameObject Etc_returnObj;
    [SerializeField] Transform  Etc_targetObj;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public void ANM_ETC_Open(GameObject _returnObj, Transform _targetObj)
    {
        this.transform.localScale = Vector3.one;
        Etc_returnObj = _returnObj;
        Etc_targetObj = _targetObj;
    }

    public void ANM_ETC_Exit()
    {
        this.transform.localScale = Vector3.zero;
        Etc_returnObj.SetActive(true);
        Etc_targetObj = null;
    }

    ////////// Unity            //////////
    void ANM_ETC_Update()
    {
        if(Etc_targetObj != null)
        {
            this.transform.position = Etc_targetObj.position;
            this.transform.rotation = Etc_targetObj.rotation;
        }
    }
}