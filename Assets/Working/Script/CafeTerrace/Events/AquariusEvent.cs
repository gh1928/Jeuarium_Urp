using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AquariusEvent : CaffeEventDeafault
{
    public Aquarius aquariusPrefab;
    private Aquarius instancedAquarius;
    public Transform aquariusPivot;

    public GameObject trailPrefab;
    public float waitBeforeTrans = 3f;
    public float waitAfterTrans = 1f;

    public Material aquariusMaterial;
    private string materialOpacity = "Opaticity";
    public float opacityChangeTime = 2f;

    public override void OnEvent(List<PuzzleElement> elements)
    {
        instancedAquarius = Instantiate(aquariusPrefab, aquariusPivot);
        instancedAquarius.transform.localPosition = Vector3.zero;
        instancedAquarius.transform.localEulerAngles = Vector3.zero;
        instancedAquarius.gameObject.SetActive(false);

        for (int i = 0; i < elements.Count; i++)
        {
            var startPos = elements[i].transform.position - elements[i].transform.forward * 0.3f;

            var trail = Instantiate(trailPrefab, startPos, Quaternion.identity);
            StartCoroutine(TrailTransCoroutine(trail, instancedAquarius.starPositions[i]));
        }

        Invoke(nameof(AquariusActiveEvent), totalEvtTime - opacityChangeTime);
    }

    private IEnumerator TrailTransCoroutine(GameObject trail, Transform target)
    {
        yield return new WaitForSeconds(waitBeforeTrans);

        Transform trailTrasform = trail.transform;
        Vector3 sourPos = trailTrasform.position;
        Vector3 destPos = target.position;

        float timer = 0f;
        float trailTime = totalEvtTime - (waitBeforeTrans + waitAfterTrans + opacityChangeTime);

        float inverseTrailTime = 1 / trailTime;

        while (timer < trailTime)
        {
            trailTrasform.position = Vector3.Lerp(sourPos, destPos, timer * inverseTrailTime);
            timer += Time.deltaTime;

            yield return null;
        }

        trailTrasform.position = destPos;        
    }
    private void AquariusActiveEvent()
    {
        instancedAquarius.gameObject.SetActive(true);
        StartCoroutine(AquariusChangeOpacity());
    }

    private IEnumerator AquariusChangeOpacity()
    {
        float timer = 0f;

        while(timer < opacityChangeTime)
        {
            aquariusMaterial.SetFloat(materialOpacity, timer/opacityChangeTime);
            timer += Time.deltaTime;
            yield return null;
        }

        aquariusMaterial.SetFloat(materialOpacity, 1);
    }
}
