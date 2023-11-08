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

        ////////// Getter & Setter  //////////
        public PaintBarrelTrigger   ANM_Basic_paintBarrelTrigger    { get { return Basic_paintBarrelTrigger;    }   }

        public Color    ANM_Basic_color { get { return Basic_color; }   }

        ////////// Method           //////////

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
    }

    ////////// Unity            //////////
    void ANM_Brush_Update()
    {
        Brush_tools.position = Brush_painter.position;
        Brush_tools.rotation = Brush_painter.rotation;
    }
}

partial class ANM_GoghYellowhouse_Manager
{
    [Header("KNIFW ==================================================")]
    [SerializeField] MeshRenderer   Knife_mixMat;
    [SerializeField] MeshRenderer   Knife_knifePaintMR;

    [Header("RUNNING")]
    [SerializeField] float Knife_red;
    [SerializeField] float Knife_green;
    [SerializeField] float Knife_blue;

    [SerializeField] float Knife_max;

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
        Knife_red += color1.r;
        if (Knife_red > Knife_max)  { Knife_max = Knife_red;    }

        Knife_green += color1.g;
        if(Knife_green > Knife_max) { Knife_max = Knife_green;  }

        Knife_blue  += color1.b;
        if (Knife_blue > Knife_max) { Knife_max = Knife_blue;   }

        //
        Color color0 = new Color(Knife_red / Knife_max, Knife_green / Knife_max, Knife_blue / Knife_max, 1.0f);

        Knife_mixMat.material.SetColor("_BaseColor", color0);
    }

    ////////// Unity            //////////
}

partial class ANM_GoghYellowhouse_Manager
{
    public enum Hand_Type
    {
        LEFT    = 0,
        RIGHT
    }

    [Header("HAND_CONTROLLER ==================================================")]
    [SerializeField] Transform Player_body;
    [SerializeField] List<BNG.HandController> Hand_hands;

    ////////// Getter & Setter  //////////
    public Transform ANM_Player_body { get { return Player_body; } }

    ////////// Method           //////////
    public bool ANM_Hand_GrabRelease(Hand_Type _type, GameObject _obj)
    {
        bool res = false;

        //
        if (Hand_hands[(int)_type].GripAmount > 0)
        {
            if ((Hand_hands[(int)_type].PreviousHeldObject != null) && (Hand_hands[(int)_type].PreviousHeldObject.Equals(_obj)))
            {
                Hand_hands[(int)_type].grabber.TryRelease();
                res = true;
            }

            if ((Hand_hands[(int)_type].grabber.RemoteGrabbingGrabbable != null) && (Hand_hands[(int)_type].grabber.RemoteGrabbingGrabbable.Equals(_obj.GetComponent<BNG.Grabbable>())))
            {
                Hand_hands[(int)_type].grabber.resetFlyingGrabbable();
                res = true;
            }
        }

        //
        return res;
    }

    ////////// Unity            //////////
}