using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternLightEvent : CaffeEventDeafault
{
    public GameObject trailPrefab;

    public Light[] lights;

    public float waitBeforeTrans = 3f;
    public float waitAfterTrans = 1f;

    public float intensity = 2f;
    public float increaseTime = 2f;

    private Coroutine lightOnCoroutine;

    private void Start()
    {
        SetLightsInetnsity(0f);
    }
    public override void OnEvent(List<PuzzleElement> elements)
    {
        //foreach (PuzzleElement element in elements)
        //{
        //    var startPos = element.transform.position - element.transform.forward * 0.3f;

        //    var trail = Instantiate(trailPrefab, startPos, Quaternion.identity);
        //    StartCoroutine(TrailTransCoroutine(trail));
        //}        

        for(int i =0; i < elements.Count; i++)
        {
            var startPos = elements[i].transform.position - elements[i].transform.forward * 0.3f;
            var trail = Instantiate(trailPrefab, startPos, Quaternion.identity);
            StartCoroutine(TrailTransCoroutine(trail, lights[1 - i].transform));
        }
    }

    private IEnumerator TrailTransCoroutine(GameObject trail, Transform target)
    {
        yield return new WaitForSeconds(waitBeforeTrans);

        Transform trailTrasform = trail.transform;
        Vector3 sourPos = trailTrasform.position;
        Vector3 destPos = target.position;

        float timer = 0f;
        float trailTime = totalEvtTime - waitBeforeTrans - waitAfterTrans;

        float inverseTrailTime = 1 / trailTime;

        while(timer < trailTime)
        {
            trailTrasform.position = Vector3.Lerp(sourPos, destPos, timer * inverseTrailTime);
            timer += Time.deltaTime;

            yield return null;
        }

        trailTrasform.position = destPos;
        Destroy(trail);



        if (lightOnCoroutine == null)
            lightOnCoroutine = StartCoroutine(IncreaseLigthIntensityCoroutine());

        yield break;
    }

    private IEnumerator IncreaseLigthIntensityCoroutine()
    {
        float timer = 0f;
        float recipIncreaseTime = 1 / increaseTime;

        while(timer < increaseTime)
        {
            float newIntensity = Mathf.Lerp(0f, intensity, timer * recipIncreaseTime);
            SetLightsInetnsity(newIntensity);
            timer += Time.deltaTime;
            yield return null;
        }

        SetLightsInetnsity(intensity);

        yield break;
    }

    private void SetLightsInetnsity(float newIntensity)
    {
        foreach(var light in lights)
        {
            light.intensity = newIntensity;
        }
    }
}
