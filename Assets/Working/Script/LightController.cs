using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private Light light;

    void Start()
    {
        light = GetComponent<Light>();
    }

    public void LightIntensityPlus()
    {
        StartCoroutine(LightIntensityPlusCoroutine());
    }

    IEnumerator LightIntensityPlusCoroutine()
    {
        WaitForSeconds time = new WaitForSeconds(0.02f);
        while (light.intensity < 1.5f)
        {
            light.intensity += Time.deltaTime;
            yield return time;
        }
    }
}
