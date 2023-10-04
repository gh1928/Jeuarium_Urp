using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternLightEvent : MonoBehaviour, Ievent
{
    public GameObject trailPrefab;
    public Transform target;

    public float waitBeforeTrans = 3f;
    public float totalEvtTime = 15f;
    public void OnEvent(List<PuzzleElement> elements)
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
        float inverseEvtTime = 1 / totalEvtTime;

        while(timer < totalEvtTime)
        {
            trailTrasform.position = Vector3.Lerp(sourPos, destPos, timer * inverseEvtTime);
            timer += Time.deltaTime;

            yield return null;
        }

        trailTrasform.position = destPos;
        Destroy(trail, 5f);
    }
}
