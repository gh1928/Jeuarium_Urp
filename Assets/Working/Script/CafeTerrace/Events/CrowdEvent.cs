using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdEvent : CaffeEventDeafault
{
    public GameObject crowdsHolder;

    public GameObject[] crowds;
    public float opacityChangeTime = 3f;

    public List<Material> crowdsMaterials = new List<Material>();
    private void Start()
    {
        SetMaterials();
        ChangeMaterialsSurfaceType(1f);
        ChangeCrowdsOpacity(0f);
    }
    //private void AddMaterialsToList(GameObject go,List<Material> materials)
    //{
    //    var renders = go.GetComponentsInChildren<Renderer>();

    //    for(int i = 0; i < renders.Length;i++)
    //    {
    //        materials.AddRange(renders[i].materials);
    //    }
    //}
    private void SetMaterials()
    {
        var renderers = crowdsHolder.GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in renderers)
        {
            crowdsMaterials.Add(renderer.material);
        }
    }

    private void ChangeCrowdsOpacity(float opacity)
    {
        for(int i = 0; i < crowdsMaterials.Count; i++)
        {
            var color = crowdsMaterials[i].color;
            color.a = opacity;
            crowdsMaterials[i].color = color;
        }
    }
    private void ChangeMaterialsSurfaceType(float type)
    {
        for (int i = 0; i < crowdsMaterials.Count; i++)
        {
            crowdsMaterials[i].SetFloat("_Surface", type);
        }
    }
    public override void OnEvent(List<PuzzleElement> _)
    {
        StartCoroutine(IncreaseOpacityCoroutine());
    }

    private IEnumerator IncreaseOpacityCoroutine()
    {
        float timer = 0f;
        float reverseChangeTime = 1 / opacityChangeTime;

        ChangeMaterialsSurfaceType(1f);

        while (timer < opacityChangeTime)
        {
            ChangeCrowdsOpacity(timer * reverseChangeTime);
            timer += Time.deltaTime;
            yield return null;
        }

        //ChangeMaterialsSurfaceType(0f);
        ChangeCrowdsOpacity(1f);
    }

}
