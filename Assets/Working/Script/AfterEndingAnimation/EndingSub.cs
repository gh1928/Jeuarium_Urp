using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndingSub : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private TextMeshProUGUI textMesh;

    public BNG.ScreenFader screenFader;

    public float fadeTime = 1f;

    public string[] subs;
    private int idx = 0;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void DoFadeIn()
    {
        screenFader.DoFadeIn();
        StartCoroutine(TextFadeCoroutine(true));

        textMesh.text = subs[idx++];
    }
    public void DoFadeOut()
    {
        screenFader.DoFadeOut();
        StartCoroutine(TextFadeCoroutine(false));
    }
    IEnumerator TextFadeCoroutine(bool fadeIn)
    {
        canvasGroup.alpha = fadeIn ? 0f : 1.0f;

        if (fadeIn)
            yield return new WaitForSeconds(2f);

        float timer = 0f;

        while (timer < fadeTime)
        {
            float value = timer / fadeTime;
            canvasGroup.alpha = fadeIn ? value : 1f - value;

            timer += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = fadeIn ? 1f : 0.0f;
    }
}
