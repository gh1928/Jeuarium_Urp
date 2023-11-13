using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AquariusElement : PuzzleElement
{
    public float blinkTime = 0.5f;
    public int blinkCount = 2;

    public Color workedColor;
    public Color failedColor;
    public Color baseColor;

    private bool readyPlaySound = true;
    private float soundCoolDown = 1f;
    private float startSoundCoolDown = 1f;
    public override void OnWorked()
    {
        if (readyPlaySound)
            PuzzlePlayer.Instance.PlayElementSound();

        base.OnWorked();
        mainImage.color = workedColor;
    }
    public override void OffWorked()
    {
        startSoundCoolDown = Time.time;

        base.OffWorked();
        mainImage.color = baseColor;
    }
    public override void PlayFailEffect() => StartCoroutine(FailEffectCoroutine());
    private void Update()
    {
        readyPlaySound = (Time.time - startSoundCoolDown) > soundCoolDown;
    }
    private IEnumerator FailEffectCoroutine()
    {
        int count = 0;
        bool colorSwitcher = false;
        float inverseBlinkTime = 1 / blinkTime;

        while (count < blinkCount)
        {
            colorSwitcher = !colorSwitcher;

            Color sourColor = colorSwitcher ? baseColor : failedColor;
            Color destColor = colorSwitcher ? failedColor : baseColor;

            float timer = 0f;
            while (timer < blinkTime)
            {
                mainImage.color = Color.Lerp(sourColor, destColor, timer * inverseBlinkTime);

                timer += Time.deltaTime;
                yield return null;
            }

            count++;
        }

        yield break;
    }
}
