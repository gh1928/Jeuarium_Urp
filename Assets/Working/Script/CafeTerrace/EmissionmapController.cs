using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionmapController
{
    private Material material; 
    public void SetMaterial(Material material) => this.material = material;
    public void SetEmissionColor(Color color) => material.SetColor("_EmissionColor", color);
    public void SetEmissionIntensity(float newIntensity) => material.SetFloat("_EmissionIntensity", newIntensity);
    public void SetEmissionColorOnly(Color color)
    {
        Color newColor = material.GetColor("_EmissionColor");
        newColor.r = color.r;
        newColor.g = color.g;  
        newColor.b = color.b;
        material.SetColor("_EmissionColor", newColor);
    }

    public IEnumerator ChageEmissionColorCoroutine(float time, Color sourColor, Color destColor)
    {
        float timer = 0f;

        float recipTime = 1 / time;
        float halfPi = Mathf.PI * 0.5f;

        while (timer < time)
        {
            float t = timer * recipTime;
            //t = Mathf.Sin(t * halfPi);

            Color newColor = Color.Lerp(sourColor, destColor, t);
            material.SetColor("_EmissionColor", newColor);
            timer += Time.deltaTime;
            yield return null;
        }

        material.SetColor("_EmissionColor", destColor);

        yield break;
    }
    public IEnumerator IncreaseEmissionCoroutine(float time, float sourValue, float destValue)
    {
        float timer = 0f;

        float recipTime = 1 / time;
        float halfPi = Mathf.PI * 0.5f;

        while (timer < time)
        {
            float t = timer * recipTime;

            //t = Mathf.Sin(t * halfPi);

            float newIntensity = Mathf.Lerp(sourValue, destValue, t);
            material.SetColor("_EmissionColor", Color.white * newIntensity);
            //material.SetFloat("_EmissionIntensity", newIntensity);
            timer += Time.deltaTime;
            yield return null;
        }

        material.SetFloat("_EmissionIntensity", destValue);

        yield break;
    }
}
