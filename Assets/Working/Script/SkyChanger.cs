using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyChanger : MonoBehaviour
{
    public Material skyMat;
    public Material glassMat;
    public Light lantern;
    public Light skyLight;
    public List<MeshRenderer> listMesh;
    public GameObject lights;
    public Animator stars;

    private void Start()
    {
        skyMat.SetFloat("_AtmosphereThickness", 0.5f);
        skyMat.SetColor("_SkyTint", new Color(0.3f, 0.3f, 0.3f, 1f));
        glassMat.SetColor("_Color", new Color(0.35f, 0.5f, 0.8f, 1f));
        glassMat.SetColor("_EmissionColor", Color.black);
    }

    public void ChangeSky()
    {
        StartCoroutine(SkyColorChange());

        lights.SetActive(true);
        stars.SetTrigger("Done");
    }

    IEnumerator SkyColorChange()
    {
        while (skyMat.GetColor("_SkyTint") != new Color(0.65f, 0.8f, 1f, 1f))
        {
            skyMat.SetFloat("_AtmosphereThickness", Mathf.Lerp(skyMat.GetFloat("_AtmosphereThickness"), 0.2f, Time.deltaTime));
            skyMat.SetColor("_SkyTint", Color.Lerp(skyMat.GetColor("_SkyTint"), new Color(0.65f, 0.8f, 1f, 1f), Time.deltaTime));
            glassMat.SetColor("_Color", Color.Lerp(glassMat.GetColor("_Color"), new Color(1.0f, 1.0f, 1.0f, 0.5f), Time.deltaTime));
            glassMat.SetColor("_EmissionColor", Color.Lerp(glassMat.GetColor("_EmissionColor"), new Color(1.0f, 0.68f, 0.3f), Time.deltaTime));
            lantern.intensity = Mathf.Lerp(lantern.intensity, 10f, Time.deltaTime);
            skyLight.intensity = Mathf.Lerp(skyLight.intensity, 0.2f, Time.deltaTime);

            yield return null;
        }
    }
}
