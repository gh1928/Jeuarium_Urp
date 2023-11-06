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
    [Header("BRUSH ==================================================")]
    [SerializeField] Transform Brush_tools;
    [SerializeField] Transform Brush_painter;

    [Header("RUNNING")]
    [SerializeField] PaintBarrelTrigger Brush_selectPaint;

    ////////// Getter & Setter  //////////
    public PaintBarrelTrigger ANM_Brush_selectPaint { set { Brush_selectPaint = value;  }   }

    ////////// Method           //////////
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

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public void ANM_Knife_SelectMat(Material _selectMat)
    {
        Knife_knifePaintMR.material = _selectMat;
    }

    public void ANM_Knife_Mix()
    {
        Color color0 = Knife_mixMat.material.GetColor(      "_BaseColor");
        Color color1 = Knife_knifePaintMR.material.GetColor("_BaseColor");

        color0.r = (color0.r + color1.r) * 0.5f;
        color0.g = (color0.g + color1.g) * 0.5f;
        color0.b = (color0.b + color1.b) * 0.5f;
        color0.a = 1.0f;
        Knife_mixMat.material.SetColor("_BaseColor", color0);
    }

    ////////// Unity            //////////
}