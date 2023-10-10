using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternLightEvent : CaffeEventDeafault
{
    public GameObject trailPrefab;
    public Transform target;

    public float waitBeforeTrans = 3f;
    public float waitAfterTrans = 1f;
    public override void OnEvent(List<PuzzleElement> elements)
    {
        foreach (PuzzleElement element in elements)
        {
            var startPos = element.transform.position - element.transform.forward * 0.3f;

            var trail = Instantiate(trailPrefab, startPos, Quaternion.identity);
            StartCoroutine(TrailTransCoroutine(trail));
        }        
    }

    private IEnumerator TrailTransCoroutine(GameObject trail)
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
        Destroy(trail, 5f);
    }
}
