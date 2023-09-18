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
    /// 저장된 그림을 불러옵니다. 해당 경로에 파일이 없다면 PlayerPrefs의 키를 삭제합니다.
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
    /// 수정/삭제 UI를 활성화합니다. DeleteDrawingUI에 num과 load의 값을 넘겨줍니다.
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
