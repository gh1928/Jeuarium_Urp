using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;

public class PaintUI : MonoBehaviour
{
    public GameObject canvas;

    [Tooltip("UI ���� ����� �귯���� �����մϴ�")]
    public BNG.DrawingTool brush;

    public BNG.DrawingTool pointBrush;

    [Tooltip("�귯���� ����� Material�� �����մϴ�")]
    public Material brushMat;

    public CustomRenderTexture origin_Tex;

    [Tooltip("Hue �����̴��� ����� ��� �̹����� �����մϴ�")]
    public RawImage image_Hue;
    [Tooltip("Saturation �����̴��� ����� ��� �̹����� �����մϴ�")]
    public RawImage image_Sat;
    [Tooltip("Value �����̴��� ����� ��� �̹����� �����մϴ�")]
    public RawImage image_Val;
    [Tooltip("�귯�� ����� ��Ÿ���� �ؽ�Ʈ UI�� �����մϴ�")]
    public TextMeshProUGUI text_Size;

    [Tooltip("Hue �����̴��� �����մϴ�")]
    public Slider slider_Hue;
    [Tooltip("Saturation �����̴��� �����մϴ�")]
    public Slider slider_Sat;
    [Tooltip("Value �����̴��� �����մϴ�")]
    public Slider slider_Val;
    [Tooltip("Alpha �����̴��� �����մϴ�")]
    public Slider slider_Alpha;
    [Tooltip("Size �����̴��� �����մϴ�")]
    public Slider slider_Size;

    [Tooltip("���� �̸� �����ִ� �̹����� �����մϴ�")]
    public Image preview;

    [Tooltip("�ؽ����� �ػ󵵸� ���մϴ�")]
    public int texRes = 300;

    Texture2D mod_Tex;

    Vector3 canvas_Size;
    Vector3 canvas_worldSize;
    Vector3 canvas_Pos;

    Color main_Color;
    Color32[] origin;

    List<Texture2D> list_Undo;
    int index = 0;

    public delegate void ColorCallBack(Color32 bColor);
    public ColorCallBack colorUpdate = null; 

    private void Awake()
    {
        if (File.Exists(Application.persistentDataPath + "/Save_Data/modification.png"))
        {
            Debug.Log("Modification Loaded");
            mod_Tex = new Texture2D(origin_Tex.width, origin_Tex.height, TextureFormat.RGB24, false);
            var data = File.ReadAllBytes(Application.persistentDataPath + "/Save_Data/modification.png");
            mod_Tex.LoadImage(data);
            origin_Tex.initializationTexture = mod_Tex;
        }
        else
            origin_Tex.initializationTexture = null;
    }

    void Start()
    {
        list_Undo = new List<Texture2D>();

        for (int i = 0; i < 20; i++)
        {
            list_Undo.Add(new Texture2D(origin_Tex.width, origin_Tex.height, TextureFormat.RGBA32, false));
        }

        RenderTexture.active = origin_Tex;
        list_Undo[0].ReadPixels(new Rect(0, 0, origin_Tex.width, origin_Tex.height), 0, 0);
        list_Undo[0].Apply();

        StartCoroutine(Initialize());                   // Initialize() �ڷ�ƾ ����
    }

    /// <summary>
    /// Saturation ���� ������ �� ����Ǵ� �Լ��Դϴ�. �����̴� ��� �̹����� ������Ʈ�մϴ�.
    /// </summary>
    public void Change_Sat()
    {
        Texture2D tex_Sat;
        if (image_Sat.texture == null)
            tex_Sat = new Texture2D(texRes, texRes);
        else tex_Sat = image_Sat.texture as Texture2D;
        for (int i = 0; i < texRes; i++)
        {
            for (int j = 0; j < texRes; j++)
            {
                tex_Sat.SetPixel(j, i, Color.HSVToRGB(slider_Hue.value, 1 - j / (float)texRes, 1.0f));
            }
        }
        tex_Sat.Apply();
        image_Sat.texture = tex_Sat;
    }

    /// <summary>
    /// Value ���� ������ �� ����Ǵ� �Լ��Դϴ�. �����̴� ��� �̹����� ������Ʈ�մϴ�.
    /// </summary>
    public void Change_Val()
    {
        Texture2D tex_Val;
        if (image_Val.texture == null)
            tex_Val = new Texture2D(texRes, texRes);
        else tex_Val = image_Val.texture as Texture2D;
        for (int i = 0; i < texRes; i++)
        {
            for (int j = 0; j < texRes; j++)
            {
                tex_Val.SetPixel(j, i, Color.HSVToRGB(slider_Hue.value, slider_Sat.value, 1 - j / (float)texRes));
            }
        }
        tex_Val.Apply();
        image_Val.texture = tex_Val;
    }

    /// <summary>
    /// Size ���� ������ �� ����Ǵ� �Լ��Դϴ�. ����� �����մϴ�.
    /// </summary>
    public void Change_Size()
    {
        brush.brushSize = slider_Size.value;
        pointBrush.brushSize = slider_Size.value;
        text_Size.text = slider_Size.value.ToString("N2");
    }

    /// <summary>
    /// �� �̸����� �̹����� ������Ʈ�ϴ� �Լ��Դϴ�. �귯�� Material�� ���� �����ϰ� ������Ʈ�մϴ�.
    /// </summary>
    public void Preview_Update()
    {
        main_Color = Color.HSVToRGB(slider_Hue.value, slider_Sat.value, slider_Val.value);
        main_Color.a = slider_Alpha.value;
        Color32 byte_Color = main_Color;
        preview.color = main_Color;

        if (brushMat.GetTexture("_DrawTex") != null)
        {
            Texture2D tempTex = brushMat.GetTexture("_DrawTex") as Texture2D;
            Color32[] tempColor = tempTex.GetPixels32();
            for (int i = 0; i < tempColor.Length; i++)
            {
                tempColor[i].r = byte_Color.r;
                tempColor[i].g = byte_Color.g;
                tempColor[i].b = byte_Color.b;
                tempColor[i].a = (byte)(origin[i].a * slider_Alpha.value);
            }
            tempTex.SetPixels32(tempColor);
            tempTex.Apply();
            brushMat.SetTexture("_DrawTex", tempTex);
        }

        if (pointBrush.mat.GetTexture("_DrawTex") != null)
        {
            Texture2D tempTex = pointBrush.mat.GetTexture("_DrawTex") as Texture2D;
            Color32[] tempColor = tempTex.GetPixels32();
            for (int i = 0; i < tempColor.Length; i++)
            {
                tempColor[i].r = byte_Color.r;
                tempColor[i].g = byte_Color.g;
                tempColor[i].b = byte_Color.b;
            }
            tempTex.SetPixels32(tempColor);
            tempTex.Apply();
            pointBrush.mat.SetTexture("_DrawTex", tempTex);
        }
    }

    /// <summary>
    /// �׷��� �׸��� �����մϴ�. ���� ��θ� PlayerPrefs�� ����մϴ�. 20���� ��� ����Ǿ��ִٸ� ���� ������ �׸����� �����ϰ� ������ Ű�� �߰��մϴ�.
    /// </summary>
    public void Save_File()
    {
        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath + "/Save_Data/");
        if (!di.Exists)
        {
            di.Create();
        }

        Texture2D tex = new Texture2D(origin_Tex.width, origin_Tex.height, TextureFormat.RGB24, false);
        RenderTexture.active = origin_Tex;
        tex.ReadPixels(new Rect(0, 0, origin_Tex.width, origin_Tex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        var data = tex.EncodeToPNG();
        string date = System.DateTime.Now.ToString("yyyy-MM-dd tt hh�� mm�� ss��");
        File.WriteAllBytes(Application.persistentDataPath + "/Save_Data/" + date + ".png", data);
        for (int i = 1; i <= 20; i++)
        {
            if (!PlayerPrefs.HasKey(i.ToString()))
            {
                PlayerPrefs.SetString(i.ToString(), date);
                break;
            }
            else if (i == 20)
            {
                for (int j = 1; j < 20; j++)
                {
                    string temp = PlayerPrefs.GetString(j.ToString());
                    File.Delete(Application.persistentDataPath + "/Save_Data/" + temp + ".png");
                    PlayerPrefs.SetString(j.ToString(), PlayerPrefs.GetString((j + 1).ToString()));
                }
                PlayerPrefs.SetString(i.ToString(), date);
                break;
            }
        }

        StartCoroutine(Save_Complete());
    }

    public void ScaleCanvas(float mag)
    {
        canvas.transform.position = canvas_Pos;
        canvas.transform.localScale = new Vector3(mag * canvas_Size.x, mag * canvas_Size.y, canvas_Size.z);
        canvas.transform.position += (mag - 1.0f) * canvas_worldSize.y * 0.5f * canvas.transform.up;
        brushMat.SetFloat("_Resolution", mag);
    }

    public void ClearDraw()
    {
        SaveHistory();

        CustomRenderTexture rt = brushMat.GetTexture("_MainTex") as CustomRenderTexture;
        rt.initializationTexture = null;
        rt.Initialize();
    }

    public void SaveHistory()
    {
        if (index >= 20)
        {
            list_Undo.RemoveAt(0);
            list_Undo.Add(new Texture2D(origin_Tex.width, origin_Tex.height, TextureFormat.RGBA32, false));

            index--;
        }

        RenderTexture.active = origin_Tex;
        list_Undo[index].ReadPixels(new Rect(0, 0, origin_Tex.width, origin_Tex.height), 0, 0);
        list_Undo[index].Apply();
        RenderTexture.active = null;

        index++;
        Debug.Log(index);
    }

    public void UndoHistory()
    {
        origin_Tex.initializationTexture = list_Undo[Mathf.Max(index - 1, 0)];
        origin_Tex.initializationColor = new Color(1, 1, 1, 1);
        origin_Tex.Initialize();

        if (index > 0)
            index--;
    }

    /// <summary>
    /// Hue �����̴� ��� �̹����� �׸���, �� �����̴��� ��� �̹����� ������Ʈ�մϴ�. �ʱ� ���� �귯���� �����մϴ�.
    /// </summary>
    /// <returns></returns>
    IEnumerator Initialize()
    {
        canvas_Size = canvas.transform.localScale;
        canvas_worldSize = 0.05f * Vector3.Scale(canvas.transform.localScale, canvas.GetComponent<MeshFilter>().mesh.bounds.size);
        canvas_Pos = canvas.transform.position;

        origin_Tex.Initialize();

        yield return new WaitUntil(() => { return brush.isReady && pointBrush.isReady; });
        origin = (brushMat.GetTexture("_DrawTex") as Texture2D).GetPixels32();

        Preview_Update();
        Change_Size();

        float div = texRes / 6.0f;
        Texture2D tex_Hue = new Texture2D(texRes, texRes);
        for (int i = 0; i < texRes; i++)
        {
            for (int j = 0; j < texRes; j++)
            {
                tex_Hue.SetPixel(j, i, new Color(
                    Mathf.Max(Mathf.Max(Mathf.Min(2.0f - (j / div), 1.0f), 0f), (j / div) - 4.0f),
                    Mathf.Max(Mathf.Min(Mathf.Min((j / div), 1.0f), 4.0f - (j / div)), 0f),
                    Mathf.Min(Mathf.Min(Mathf.Max((j / div) - 2.0f, 0f), 1.0f), 6.0f - (j / div))
                    ));
            }
        }
        tex_Hue.Apply();
        image_Hue.texture = tex_Hue;
        Change_Sat();
        Change_Val();

        if (File.Exists(Application.persistentDataPath + "/Save_Data/modification.png"))
        {
            File.Delete(Application.persistentDataPath + "/Save_Data/modification.png");
        }

        yield return null;
    }

    IEnumerator Save_Complete()
    {
        GameObject btn = EventSystem.current.currentSelectedGameObject;
        btn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "���� �Ϸ�!";

        yield return new WaitForSeconds(1.0f);

        btn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "����";

        yield return null;
    }
}
