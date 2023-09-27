using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanternElement : PuzzleElement
{
    public Image lanternLight;

    public Color workedColor;
    public Color failedColor;
    public Color baseColor;
    protected override void OnWorked()
    {
        base.OnWorked();
        mainImage.color = workedColor;
        lanternLight.color = workedColor;
    }
    protected override void OffWorked()
    {
        base.OffWorked();
        mainImage.color = baseColor;
        lanternLight.color = baseColor;
    }
}
