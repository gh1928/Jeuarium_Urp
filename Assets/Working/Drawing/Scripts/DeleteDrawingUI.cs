using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class DeleteDrawingUI : MonoBehaviour
{
    [HideInInspector]
    public int num;
    [HideInInspector]
    public LoadDrawing load;

    [Tooltip("가상실습실에서 사용될 커스텀 렌더 텍스처를 지정합니다.")]
    public CustomRenderTexture rt;

    /// <summary>
    /// 그림을 저장하는 폴더가 존재하는 지 검사하고, 생성합니다.
    /// </summary>
    private void Awake()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Save_Data"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Save_Data");
        }

        if (File.Exists(Application.persistentDataPath + "/Save_Data/modification.png"))
        {
            File.Delete(Application.persistentDataPath + "/Save_Data/modification.png");
        }
    }

    private void OnEnable()
    {
        rt.initializationTexture = null;

        StartCoroutine(UIDisable());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator UIDisable()
    {
        yield return new WaitForSeconds(3.0f);

        gameObject.SetActive(false);

        yield break;
    }

    /// <summary>
    /// PlayerPrefs 에서 num과 일치하는 키를 가진 그림이 존재하는 지 확인하고, 삭제합니다.
    /// </summary>
    public void DeleteDraw()
    {
        if (PlayerPrefs.HasKey(num.ToString()))
        {
            File.Delete(Application.persistentDataPath + "/Save_Data/" + PlayerPrefs.GetString(num.ToString()) + ".png");
            PlayerPrefs.DeleteKey(num.ToString());
            load.LoadDraw();
        }
    }

    /// <summary>
    /// PlayerPrefs 에서 num과 일치하는 키를 가진 그림이 존재하는 지 확인하고, 파일로 저장하여 가상실습실에서 수정합니다.
    /// </summary>
    public void ModifyDraw()
    {
        if (PlayerPrefs.HasKey(num.ToString()))
        {
            var data = load.tex.EncodeToPNG();
            File.WriteAllBytes(Application.persistentDataPath + "/Save_Data/modification.png", data);

            File.Delete(Application.persistentDataPath + "/Save_Data/" + PlayerPrefs.GetString(num.ToString()) + ".png");
            PlayerPrefs.DeleteKey(num.ToString());
            SceneManager.LoadScene("Studio");
        }
    }
}
