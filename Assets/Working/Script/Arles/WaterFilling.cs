using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFilling : MonoBehaviour
{
    [Tooltip("병 뚜껑 오브젝트를 지정합니다.")]
    public GameObject cap;
    [Tooltip("병 입구의 지름을 기입합니다. Sphere Collider를 임의로 배치해 측정할 수 있습니다.")]
    public float rad = 0.0f;

    AudioSource audioSource;
    Renderer rd;
    Material mt;
    Mesh ms;
    float objHeight = 0.1f;
    float surfaceHeight = 0.0f;
    bool isOpen = false;
    bool isPouring = false;
    public bool IsOpen { get { return isOpen; } set { isOpen = value; } }
    public bool IsPouring { get { return isPouring; } }
    //
    public Color WaterColor { get { return mt.GetColor(mt_Color_Name); } }
    //public Color WaterColor { get { return mt.color; } }
    //
    public Material Mat { get { return mt; } }

    //
    string  mt_TopPos_Name          = "Vector3_e8ca6f7e74f7492fb1e98746fde22a39";
    string  mt_BottomPos_Name       = "Vector3_665e1dee556d46a8bfcdcfe2f3890780";
    string  mt_SurfaceHeight_Name   = "Vector1_a08a4fecbc3e4f5998da40359efdcda0";
    string  mt_Color_Name           = "Color_761631cd168a4534ad00065c72496562";
    
    float   mt_water_gageMin    = 0.0f;
    float   mt_water_gageRange  = 0.0f;

    //
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rd = GetComponent<MeshRenderer>();
        mt = rd.material;
        ms = GetComponent<MeshFilter>().mesh;

        objHeight = transform.lossyScale.y * ms.bounds.size.y;
        //
        //surfaceHeight = mt.GetFloat(mt_SurfaceHeight_Name);
        //surfaceHeight = mt.GetFloat("_SurfaceHeight");

        mt_water_gageMin    = mt.GetFloat(mt_SurfaceHeight_Name);
        mt_water_gageRange  = (0.5f - mt_water_gageMin) * 2.0f;

        ResetLiquid();

        surfaceHeight = mt.GetFloat(mt_SurfaceHeight_Name);

        StartCoroutine(Pouring());
    }

    void Update()
    {
        // 병뚜껑을 기반으로 계산합니다.
        isPouring = (cap.transform.position.y < this.transform.position.y);
        // 수면의 높이가 병 입구 하단의 높이보다 높은지 체크합니다.
        //isPouring = rd.bounds.min.y + (rd.bounds.max.y - rd.bounds.min.y) * ((surfaceHeight - mt_water_gageMin) / mt_water_gageRange) >= ((transform.position + transform.up * objHeight) + Vector3.Cross(Vector3.Cross(Vector3.up, transform.up), transform.up) * rad).y;
        //isPouring = rd.bounds.min.y + (rd.bounds.max.y - rd.bounds.min.y) * surfaceHeight >= ((transform.position + transform.up * objHeight) + Vector3.Cross(Vector3.Cross(Vector3.up, transform.up), transform.up) * rad).y;

        //
        mt.SetVector(mt_BottomPos_Name, rd.bounds.min); // 메쉬의 최하단 지점 위치를 머티리얼로 보냅니다.
        mt.SetVector(mt_TopPos_Name,    rd.bounds.max); // 메쉬의 최상단 지점 위치를 머티리얼로 보냅니다.
        //mt.SetVector("_BottomPos", rd.bounds.min);          // 메쉬의 최하단 지점 위치를 머티리얼로 보냅니다.
        //mt.SetVector("_TopPos", rd.bounds.max);             // 메쉬의 최상단 지점 위치를 머티리얼로 보냅니다.

        // 수면 높이가 0.001f 이하면 스크립트를 종료합니다.
        if (0.5f - mt.GetFloat(mt_SurfaceHeight_Name) <= mt_water_gageRange * 0.001f)
        //if (mt.GetFloat("_SurfaceHeight") <= 0.001f)
        {
            StopAllCoroutines();
            //
            Color mtColor = mt.GetColor(mt_Color_Name);
            mtColor.a = 0.0f;
            mt.SetColor(mt_Color_Name, mtColor);
            //mt.color = new Color(mt.color.r, mt.color.g, mt.color.b, 0);
            enabled = false;
        }
    }

    /// <summary>
    /// 병 안의 물을 초기화 합니다.
    /// </summary>
    public void ResetLiquid()
    {
        //
        mt.SetFloat(mt_SurfaceHeight_Name, mt_water_gageMin + (mt_water_gageRange * 0.33f));
        //mt.SetFloat("_SurfaceHeight", 0.33f);
        //
        Color mtColor = mt.GetColor(mt_Color_Name);
        mtColor.a = 0.5f;
        mt.SetColor(mt_Color_Name, mtColor);
        //mt.color = new Color(mt.color.r, mt.color.g, mt.color.b, 0.5f);
        //
        enabled = true;
    }

    /// <summary>
    /// 병 뚜껑이 열려 있고 병이 충분히 기울어진 상태일 때 수면 높이를 낮춥니다.
    /// </summary>
    /// <returns></returns>
    IEnumerator Pouring()
    {
        yield return new WaitUntil(() => { return isOpen && isPouring; });
        audioSource.Play();
        //
        mt.SetFloat(mt_SurfaceHeight_Name, mt.GetFloat(mt_SurfaceHeight_Name) - ((surfaceHeight - mt_water_gageMin) * 0.1f));
        //mt.SetFloat("_SurfaceHeight", mt.GetFloat("_SurfaceHeight") - surfaceHeight * 0.1f);
        //
        yield return new WaitUntil(() => { return !isPouring || !isOpen; });
        StartCoroutine(Pouring());
        yield return null;
    }
}
