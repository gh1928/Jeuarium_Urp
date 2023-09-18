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

    [Tooltip("����ǽ��ǿ��� ���� Ŀ���� ���� �ؽ�ó�� �����մϴ�.")]
    public CustomRenderTexture rt;

    /// <summary>
    /// �׸��� �����ϴ� ������ �����ϴ� �� �˻��ϰ�, �����մϴ�.
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
    /// PlayerPrefs ���� num�� ��ġ�ϴ� Ű�� ���� �׸��� �����ϴ� �� Ȯ���ϰ�, �����մϴ�.
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
    /// PlayerPrefs ���� num�� ��ġ�ϴ� Ű�� ���� �׸��� �����ϴ� �� Ȯ���ϰ�, ���Ϸ� �����Ͽ� ����ǽ��ǿ��� �����մϴ�.
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
