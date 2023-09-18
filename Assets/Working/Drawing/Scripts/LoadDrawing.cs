using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadDrawing : MonoBehaviour
{
    public int num;
    public GameObject ui;
    [HideInInspector]
    public Texture2D tex;

    void Start()
    {
        LoadDraw();
    }

    /// <summary>
    /// ����� �׸��� �ҷ��ɴϴ�. �ش� ��ο� ������ ���ٸ� PlayerPrefs�� Ű�� �����մϴ�.
    /// </summary>
    public void LoadDraw()
    {
        if (PlayerPrefs.HasKey(num.ToString()))
        {
            string str = PlayerPrefs.GetString(num.ToString());
            try
            {
                var data = File.ReadAllBytes(Application.persistentDataPath + "/Save_Data/" + str + ".png");
                tex = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
                tex.LoadImage(data);
                GetComponent<MeshRenderer>().material.mainTexture = tex;
            }
            catch (FileNotFoundException e)
            {
                System.Console.WriteLine(e);
                PlayerPrefs.DeleteKey(num.ToString());
            }
        }

        else
        {
            tex = new Texture2D(0, 0, TextureFormat.RGB24, false);
            GetComponent<MeshRenderer>().material.mainTexture = null;
        }
    }

    public void OpenUI()
    {
        StartCoroutine(ActiveUI());
    }

    /// <summary>
    /// ����/���� UI�� Ȱ��ȭ�մϴ�. DeleteDrawingUI�� num�� load�� ���� �Ѱ��ݴϴ�.
    /// </summary>
    /// <returns></returns>
    IEnumerator ActiveUI()
    {
        if (ui.activeSelf) 
            ui.SetActive(false);

        yield return null;

        ui.SetActive(true);
        ui.transform.position = transform.position + (0.5f * transform.right);
        ui.transform.rotation = transform.rotation;
        ui.GetComponent<DeleteDrawingUI>().num = num;
        ui.GetComponent<DeleteDrawingUI>().load = this;

        yield break;
    }
}
