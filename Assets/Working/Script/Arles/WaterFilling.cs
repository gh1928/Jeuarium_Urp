using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFilling : MonoBehaviour
{
    [Tooltip("�� �Ѳ� ������Ʈ�� �����մϴ�.")]
    public GameObject cap;
    [Tooltip("�� �Ա��� ������ �����մϴ�. Sphere Collider�� ���Ƿ� ��ġ�� ������ �� �ֽ��ϴ�.")]
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
        // ���Ѳ��� ������� ����մϴ�.
        isPouring = (cap.transform.position.y < this.transform.position.y);
        // ������ ���̰� �� �Ա� �ϴ��� ���̺��� ������ üũ�մϴ�.
        //isPouring = rd.bounds.min.y + (rd.bounds.max.y - rd.bounds.min.y) * ((surfaceHeight - mt_water_gageMin) / mt_water_gageRange) >= ((transform.position + transform.up * objHeight) + Vector3.Cross(Vector3.Cross(Vector3.up, transform.up), transform.up) * rad).y;
        //isPouring = rd.bounds.min.y + (rd.bounds.max.y - rd.bounds.min.y) * surfaceHeight >= ((transform.position + transform.up * objHeight) + Vector3.Cross(Vector3.Cross(Vector3.up, transform.up), transform.up) * rad).y;

        //
        mt.SetVector(mt_BottomPos_Name, rd.bounds.min); // �޽��� ���ϴ� ���� ��ġ�� ��Ƽ����� �����ϴ�.
        mt.SetVector(mt_TopPos_Name,    rd.bounds.max); // �޽��� �ֻ�� ���� ��ġ�� ��Ƽ����� �����ϴ�.
        //mt.SetVector("_BottomPos", rd.bounds.min);          // �޽��� ���ϴ� ���� ��ġ�� ��Ƽ����� �����ϴ�.
        //mt.SetVector("_TopPos", rd.bounds.max);             // �޽��� �ֻ�� ���� ��ġ�� ��Ƽ����� �����ϴ�.

        // ���� ���̰� 0.001f ���ϸ� ��ũ��Ʈ�� �����մϴ�.
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
    /// �� ���� ���� �ʱ�ȭ �մϴ�.
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
    /// �� �Ѳ��� ���� �ְ� ���� ����� ������ ������ �� ���� ���̸� ����ϴ�.
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
