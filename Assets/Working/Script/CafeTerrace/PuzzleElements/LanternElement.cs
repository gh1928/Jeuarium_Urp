using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanternElement : PuzzleElement
{
    public Image lanternLight;

    public float blinkTime = 0.5f;
    public int blinkCount = 2;

    public Color workedColor;
    public Color failedColor;
    public Color baseColor;
    public override void OnWorked()
    {
        base.OnWorked();
        mainImage.color = workedColor;

        lanternLight.enabled = true;
        lanternLight.color = workedColor;
    }
    public override void OffWorked()
    {
        base.OffWorked();
        mainImage.color = baseColor;

        lanternLight.enabled = false;
        //lanternLight.color = baseColor;
    }
    public override void PlayFailEffect() => StartCoroutine(FailEffectCoroutine());
    private IEnumerator FailEffectCoroutine()
    {
        int count = 0;
        bool colorSwitcher = true;        
        float inverseBlinkTime = 1 / blinkTime;

        while (count < blinkCount)
        {
            Color sourColor = colorSwitcher ? baseColor : failedColor;
            Color destColor = colorSwitcher ? failedColor : baseColor;
            
            float timer = 0f;
            while (timer < blinkTime)
            {
                mainImage.color = Color.Lerp(sourColor, destColor, timer * inverseBlinkTime);

                timer += Time.deltaTime;
                yield return null;
            }
            
            colorSwitcher = !colorSwitcher;
            count++;
        }

        yield break;
    }
}
