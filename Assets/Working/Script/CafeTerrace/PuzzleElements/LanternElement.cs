using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanternElement : PuzzleElement
{
    //public Image lanternLight;

    public float blinkTime = 0.5f;
    public int blinkCount = 2;

    public Color workedColor;
    public Color failedColor;
    public Color baseColor;

    private bool readyPlaySound = true;
    private float soundCoolDown = 1f;
    private float startSoundCoolDown = 1f;

    public float rotSpeed = 3f;

    public override void OnWorked()
    {
        if(readyPlaySound)
            PuzzlePlayer.Instance.PlayElementSound();

        base.OnWorked();
        mainImage.color = workedColor;

        //lanternLight.enabled = true;
        //lanternLight.color = workedColor;
    }
    public override void OffWorked()
    {
        startSoundCoolDown = Time.time;

        base.OffWorked();
        mainImage.color = baseColor;

        //lanternLight.enabled = false;
        //lanternLight.color = baseColor;
    }
    //public override void PlaySuccessEffect() => StartCoroutine(SuccessEffectCoroutine());
    public override void PlayFailEffect() => StartCoroutine(FailEffectCoroutine());
    private void Update()
    {
        readyPlaySound = (Time.time - startSoundCoolDown) > soundCoolDown;

        transform.Rotate(rotSpeed * Time.deltaTime * Vector3.forward);
    }

    //private IEnumerator SuccessEffectCoroutine()
    //{
    //    float totalTime = 0.5f;
    //    float inverseTotalTime = 1 / totalTime;
    //    float timer = 0f;

    //    Color transparent = workedColor;
    //    transparent.a = 0f;

    //    while(timer < totalTime)
    //    {
    //        mainImage.color = Color.Lerp(workedColor, transparent, timer * inverseTotalTime);
    //        lanternLight.color = Color.Lerp(workedColor, transparent, timer * inverseTotalTime);

    //        timer += Time.deltaTime;
    //        yield return null;
    //    }

    //    mainImage.color = transparent;
    //    lanternLight.color= transparent;
    //}
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
