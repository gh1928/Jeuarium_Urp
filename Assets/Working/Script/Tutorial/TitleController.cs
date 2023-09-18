using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    public CanvasGroup Intro;
    TMPro.TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        StartCoroutine(IntroExpose());
    }

    /// <summary>
    /// Ÿ��Ʋ�� Fade Out ó���ϰ� ������ �����մϴ�.
    /// </summary>
    /// <returns></returns>
    IEnumerator IntroExpose()
    {
        yield return new WaitForSeconds(2.0f);

        while (text.color.a > 0)
        {
            text.color -= new Color(0, 0, 0, Time.deltaTime);
            
            yield return null;
        }

        while (Intro.alpha < 1)
        {
            Intro.alpha += Time.deltaTime;

            yield return null;
        }

        yield break;
    }
}
