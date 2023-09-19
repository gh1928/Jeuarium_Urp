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