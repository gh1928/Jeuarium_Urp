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

    protected override void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter");

        base.OnTriggerEnter(other);
        mainImage.color = workedColor;
        lanternLight.color = workedColor;
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        mainImage.color = baseColor;
        lanternLight.color = baseColor;
    }
}
