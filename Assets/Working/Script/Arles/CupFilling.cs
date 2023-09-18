using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CupFilling : MonoBehaviour
{
    public Slider slider_Red;
    public Slider slider_Green;
    public Slider slider_Blue;

    public bool isOK = false;

    WaterFilling wf;

    Renderer rd;
    Material mt;

    Coroutine cor;

    int ratio = 10;
    
    //
    string  mt_TopPos_Name          = "Vector3_e8ca6f7e74f7492fb1e98746fde22a39";
    string  mt_BottomPos_Name       = "Vector3_665e1dee556d46a8bfcdcfe2f3890780";
    string  mt_Color_Name           = "Color_761631cd168a4534ad00065c72496562";
    string  mt_SurfaceHeight_Name   = "Vector1_a08a4fecbc3e4f5998da40359efdcda0";
    
    float   mt_water_gageMin    = 0.0f;
    float   mt_water_gageRange  = 0.0f;

    //
    private void Start()
    {
        rd = GetComponent<MeshRenderer>();
        mt = rd.material;
        
        mt.SetVector(mt_BottomPos_Name, rd.bounds.min); // �޽��� ���ϴ� ���� ��ġ�� ��Ƽ����� �����ϴ�.
        //mt.SetVector("_BottomPos", rd.bounds.min);      // �޽��� ���ϴ� ���� ��ġ�� ��Ƽ����� �����ϴ�.
        mt.SetVector(mt_TopPos_Name,    rd.bounds.max); // �޽��� �ֻ�� ���� ��ġ�� ��Ƽ����� �����ϴ�.
        //mt.SetVector("_TopPos", rd.bounds.max);         // �޽��� �ֻ�� ���� ��ġ�� ��Ƽ����� �����ϴ�.

        slider_Red.value    = mt.GetColor(mt_Color_Name).r;
        slider_Green.value  = mt.GetColor(mt_Color_Name).g;
        slider_Blue.value   = mt.GetColor(mt_Color_Name).b;

        //slider_Red.value = mt.color.r;
        //slider_Green.value = mt.color.g;
        //slider_Blue.value = mt.color.b;

        mt_water_gageMin    = mt.GetFloat(mt_SurfaceHeight_Name);
        mt_water_gageRange  = (0.5f - mt_water_gageMin) * 2.0f;

        ResetLiquid();

        StartCoroutine(CheckColor());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Liquid"))
        {
            //
            for(int i = 0; i < other.transform.parent.childCount; i++)
            {
                Transform childTrans = other.transform.parent.GetChild(i);
                if(childTrans.name.Split('_')[0].Equals("water") || childTrans.name.Split('_')[0].Equals("Water"))
                {
                    wf = childTrans.GetComponent<WaterFilling>();
                    break;
                }
            }
            //wf = other.transform.GetChild(0).GetComponent<WaterFilling>();
            //
            wf.cap.SetActive(false);                    // �� �Ѳ��� ��Ȱ��ȭ �մϴ�.
            wf.IsOpen = true;

            cor = StartCoroutine(Filling());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Liquid"))
        {
            wf.cap.SetActive(true);
            wf.IsOpen = false;
            wf = null;
            StopCoroutine(cor);
        }
    }

    /// <summary>
    /// �� ���� ���� �ʱ�ȭ�մϴ�.
    /// </summary>
    public void ResetLiquid()
    {
        ratio = 10;
        mt.SetFloat(mt_SurfaceHeight_Name, mt_water_gageMin + (mt_water_gageRange * 0.5f));
        //mt.SetFloat("_SurfaceHeight", 0.5f);
        mt.SetColor(mt_Color_Name, Color.white - 0.5f * Color.black);
        //mt.color = Color.white - 0.5f * Color.black;

        //
        Color mainColor = mt.GetColor(mt_Color_Name);
        slider_Red.value    = mainColor.r;
        slider_Green.value  = mainColor.g;
        slider_Blue.value   = mainColor.b;
        //slider_Red.value = mt.color.r;
        //slider_Green.value = mt.color.g;
        //slider_Blue.value = mt.color.b;
    }

    /// <summary>
    /// �� ���� �� ���� ������ ���� ��ġ�ϸ� ������ �����մϴ�.
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckColor()
    {
        yield return new WaitUntil
            (
                () =>
                {
                    return Mathf.Abs(mt.GetColor(mt_Color_Name).r - 0.9f) < 0.01f
                        && Mathf.Abs(mt.GetColor(mt_Color_Name).g - 0.7f) < 0.01f
                        && Mathf.Abs(mt.GetColor(mt_Color_Name).b - 0.4f) < 0.01f

                        && !isOK;
                }
            );
        //yield return new WaitUntil(() => { return Mathf.Abs(mt.color.r - 0.9f) < 0.01f && Mathf.Abs(mt.color.g - 0.7f) < 0.01f && Mathf.Abs(mt.color.b - 0.4f) < 0.01f && !isOK; });

        isOK = true;
        yield return null;
    }

    /// <summary>
    /// ���� ������ ���� �ξ����� �� ���� ���� ä�����ϴ�.
    /// </summary>
    /// <returns></returns>
    IEnumerator Filling()
    {
        yield return new WaitUntil(() => { return wf.IsPouring; });
        if (ratio <= 0) yield break;
        Color col = (Color.white - wf.WaterColor) * 0.1f;
        col.a = 0;
        mt.SetColor(mt_Color_Name, mt.GetColor(mt_Color_Name) - col);
        //mt.color -= col;
        mt.SetFloat(mt_SurfaceHeight_Name, mt.GetFloat(mt_SurfaceHeight_Name) + (mt_water_gageRange * 0.3f));
        //mt.SetFloat("_SurfaceHeight", mt.GetFloat("_SurfaceHeight") + 0.03f);

        //
        Color mainColor = mt.GetColor(mt_Color_Name);
        slider_Red.value    = mainColor.r;
        slider_Green.value  = mainColor.g;
        slider_Blue.value   = mainColor.b;
        //slider_Red.value = mt.color.r;
        //slider_Green.value = mt.color.g;
        //slider_Blue.value = mt.color.b;
        //
        yield return new WaitUntil(() => { return !wf.IsPouring || !wf.IsOpen; });
        cor = StartCoroutine(Filling());
        yield return null;
    }
}
