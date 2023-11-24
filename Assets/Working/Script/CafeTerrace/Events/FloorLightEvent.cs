using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLightEvent : CaffeEventDeafault
{
    public Color startColor;
    public Color endColor;

    //public float startIntensity = -4f;
    //public float endIntensity = 0f;
    public float time = 5f;

    public MeshRenderer floorRenderer;

    private EmissionmapController emissionController = new EmissionmapController();

    private void Start()
    {
        emissionController.SetMaterial(floorRenderer.material);
        emissionController.SetEmissionColor(startColor);
    }

    public override void OnEvent(List<PuzzleElement> _)
    {
        StartCoroutine(emissionController.ChageEmissionColorCoroutine(time, startColor, endColor));
    }
}
