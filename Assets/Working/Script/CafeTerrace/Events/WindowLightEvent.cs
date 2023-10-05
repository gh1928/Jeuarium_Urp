using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowLightEvent : CaffeEventDeafault
{
    public override void OnEvent(List<PuzzleElement> _)
    {
        StartCoroutine(LightOnCoroutine());
    }

    public Material windowMaterial;
    private string materialSwtich = "OnOffswitch";

    public float lightTime = 3f;

    private void Start()
    {
        windowMaterial.SetFloat(materialSwtich, 0f);
    }

    private IEnumerator LightOnCoroutine()
    {
        float timer = 0f;

        while (timer < lightTime)
        {
            windowMaterial.SetFloat(materialSwtich, timer/lightTime);
            timer += Time.deltaTime;

            yield return null;
        }

        windowMaterial.SetFloat(materialSwtich, 1f);
    }
}
