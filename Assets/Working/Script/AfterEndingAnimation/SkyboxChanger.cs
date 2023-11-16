using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public float lightIntencity = 1f;
    public void ChangeSkyBox(Material material) => RenderSettings.skybox = material;

    public void Update()
    {
        RenderSettings.ambientIntensity = lightIntencity;
    }
}
