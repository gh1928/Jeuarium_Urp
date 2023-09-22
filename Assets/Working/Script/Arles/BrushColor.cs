using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 컵에 들어있는 물의 색으로 브러쉬 색을 변경합니다.
/// </summary>
public partial class BrushColor : MonoBehaviour
{
    public GameObject cup;
    CupFilling cf;
    TriggerSet trigger;

    //
    string other_Color_name = "Color_761631cd168a4534ad00065c72496562";

    //
    private void Start()
    {
        cf = cup.GetComponent<CupFilling>();
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject == cup)
        {
            GetComponent<MeshRenderer>().material.color = _other.transform.GetComponent<MeshRenderer>().material.GetColor(other_Color_name);
        }
        //else if (_other.CompareTag("Canvas") && cf.isOK)
        //{
        //    trigger.SwitchOn();
        //}
        else
        {
            ANM_Basic_OnTriggerEnter(_other);
        }
    }

    public void SetTrigger(TriggerSet t)
    {
        t.AddTrigger();
        trigger = t;
    }
}

partial class BrushColor
{
    [Header("HYJ_ADD ==================================================")]
    [SerializeField] GameObject Basic_brush;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    void ANM_Basic_OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Paint"))
        {
            _other.GetComponent<ANM_ColorPainting>();
        }
    }

    ////////// Unity            //////////
}