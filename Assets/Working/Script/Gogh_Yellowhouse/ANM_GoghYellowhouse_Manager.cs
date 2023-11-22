using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ANM_GoghYellowhouse_Manager : MonoBehaviour
{

    ////////// Getter & Setter  //////////

    ////////// Method           //////////

    ////////// Unity            //////////
    // Start is called before the first frame update
    void Start()
    {
        ANM_Brush_Start();
    }

    // Update is called once per frame
    void Update()
    {
        ANM_Brush_Update();
    }
}

partial class ANM_GoghYellowhouse_Manager
{
    [System.Serializable]
    public class ANM_BrushSelectData
    {
        [SerializeField] PaintBarrelTrigger Basic_paintBarrelTrigger;
        [SerializeField] Color Basic_color;
        [SerializeField] string Basic_colorStr;

        public delegate string Basic_Text(int _r, int _g, int _b);

        ////////// Getter & Setter  //////////
        public PaintBarrelTrigger   ANM_Basic_paintBarrelTrigger    { get { return Basic_paintBarrelTrigger;    }   }

        public Color    ANM_Basic_color { get { return Basic_color; }   }

        public string   ANM_Basic_colorStr  { get { return Basic_colorStr;  }   }

        ////////// Method           //////////
        public void ANM_Basic_Init(Basic_Text _method_text)
        {
            float min = 1;

            float cyan      = 1.0f - Basic_color.r;
            float magenta   = 1.0f - Basic_color.g;
            float yellow    = 1.0f - Basic_color.b;
            if ((cyan       != 0.0f ) && (min > cyan    ))  { min = cyan;       }
            if ((magenta    != 0.0f ) && (min > magenta ))  { min = magenta;    }
            if ((yellow     != 0.0f ) && (min > yellow  ))  { min = yellow;     }

            Basic_colorStr = _method_text((int)(cyan / min), (int)(magenta / min), (int)(yellow / min));
        }

        ////////// Unity            //////////
    }

    [Header("BRUSH ==================================================")]
    [SerializeField] Transform Brush_tools;
    [SerializeField] Transform Brush_painter;
    [SerializeField] List<ANM_BrushSelectData> Brush_selectDatas;

    [Header("RUNNING")]
    [SerializeField] PaintBarrelTrigger Brush_selectPaint;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public void ANM_Brush_Select(Color _color)
    {
        Brush_selectPaint = null;

        for(int i = 0; i < Brush_selectDatas.Count; i++)
        {
            Color color0 = Brush_selectDatas[i].ANM_Basic_color;
            if(_color.r.Equals(color0.r) && _color.g.Equals(color0.g) && _color.b.Equals(color0.b))
            {
                Brush_selectPaint = Brush_selectDatas[i].ANM_Basic_paintBarrelTrigger;
                break;
            }
        }
    }

    public void ANM_Brush_Painting()
    {
        if(Brush_selectPaint != null)
        {
            Brush_selectPaint.ANM_Brush_Painting();
        }

        //ANM_Knife_Reset();
        ANM_Knife_Text();
    }

    ////////// Unity            //////////
    void ANM_Brush_Start()
    {
        for(int i = 0; i < Brush_selectDatas.Count; i++)
        {
            Brush_selectDatas[i].ANM_Basic_Init(ANM_Knife_Text__Value);
        }

        ANM_Knife_Text();
    }

    void ANM_Brush_Update()
    {
        //Brush_tools.position = Brush_painter.position;
        //Brush_tools.rotation = Brush_painter.rotation;
    }
}

partial class ANM_GoghYellowhouse_Manager
{
    [Header("KNIFW ==================================================")]
    [SerializeField] MeshRenderer   Knife_mixMat;
    [SerializeField] MeshRenderer   Knife_knifePaintMR;

    [SerializeField] TMPro.TextMeshProUGUI Knife_tmp;

    [Header("RUNNING")]
    [SerializeField] float Knife_cyan;
    [SerializeField] float Knife_magenta;
    [SerializeField] float Knife_yellow;

    [SerializeField] float Knife_max;

    [SerializeField] List<string> Knife_tmpStrs;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public void ANM_Knife_SelectMat(Material _selectMat)
    {
        Knife_knifePaintMR.material = _selectMat;
    }

    public void ANM_Knife_Mix()
    {
        Color color1 = Knife_knifePaintMR.material.GetColor("_BaseColor");

        //
        Knife_cyan += 1.0f - color1.r;
        if (Knife_cyan > Knife_max)  { Knife_max = Knife_cyan;    }

        Knife_magenta += 1.0f - color1.g;
        if(Knife_magenta > Knife_max) { Knife_max = Knife_magenta;  }

        Knife_yellow += 1.0f - color1.b;
        if (Knife_yellow > Knife_max) { Knife_max = Knife_yellow;   }

        //
        Color color0 = new Color(1.0f - (Knife_cyan / Knife_max), 1.0f - (Knife_magenta / Knife_max), 1.0f - (Knife_yellow / Knife_max), 1.0f);

        Knife_mixMat.material.SetColor("_BaseColor", color0);

        //
        ANM_Knife_Text();
    }

    public void ANM_Knife_Reset()
    {
        Knife_cyan = Knife_magenta = Knife_yellow = 0.0f;

        Knife_mixMat.material.SetColor("_BaseColor", Color.white);
        ANM_Knife_Text();
    }

    void ANM_Knife_Text()
    {
        Color color = Knife_mixMat.material.GetColor("_BaseColor");

        //
        if (Knife_tmp != null)
        {
            Knife_tmp.text = "";
            for (int i = 0; i < Brush_selectDatas.Count; i++)
            {
                if (Brush_selectDatas[i].ANM_Basic_paintBarrelTrigger.ANM_Basic_isDone)
                {
                    Knife_tmp.text += "б▄ ";
                }
                else if (Brush_selectDatas[i].ANM_Basic_color.Equals(color))
                {
                    Knife_tmp.text += "б▌ ";
                }
                else
                {
                    Knife_tmp.text += "б█ ";
                }
                Knife_tmp.text += Brush_selectDatas[i].ANM_Basic_colorStr + "\n";
            }
            Knife_tmp.text += "\n";
            Knife_tmp.text += ANM_Knife_Text__Value((int)Knife_cyan, (int)Knife_magenta, (int)Knife_yellow);
        }
    }

    string ANM_Knife_Text__Value(int _c, int _m, int _y)
    {
        return "<color=#00FFFF>бс</color>:" + _c + ", <color=#FF00FF>бс</color>:" + _m + ", <color=#FFFF00>бс</color>:" + _y;
    }

    ////////// Unity            //////////
}