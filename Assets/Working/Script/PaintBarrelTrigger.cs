using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBarrelTrigger : BNG.GrabbableEvents
{
    public GameObject paint;
    public GameObject obj;
    public List<Material> list_Mat;
    public bool isSky = false;

    int i = 0;
    bool isDone = false;
    ParticleSystem ps;
    TriggerSet trigger;

    public override void OnGrab(Grabber grabber)
    {
        ps.Play();

        base.OnGrab(grabber);
    }

    public override void OnRelease()
    {
        ps.Stop();

        base.OnRelease();
    }

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();

        if (isSky)
        {
            list_Mat[0].SetColor("_SkyTint", new Color(0.4f, 0.2f, 0.0f, 1.0f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Canvas") && !isDone)
        {
            trigger.SwitchOn();
            GetComponent<AudioSource>().Play();

            paint.SetActive(true);
            if (!isSky)
            {
                foreach (MeshRenderer mr in obj.transform.GetComponentsInChildren<MeshRenderer>())
                {
                    mr.materials = list_Mat.GetRange(i, mr.materials.Length).ToArray();
                    i += mr.materials.Length;
                }
            }
            else
            {
                StartCoroutine(SkyColor());
            }

            isDone = true;
            enabled = false;
        }
    }

    public void SetTrigger(TriggerSet t)
    {
        t.AddTrigger();
        trigger = t;
    }

    IEnumerator SkyColor()
    {
        Color c;

        while (list_Mat[0].GetColor("_SkyTint") != list_Mat[1].color)
        {
            c = Color.Lerp(list_Mat[0].GetColor("_SkyTint"), list_Mat[1].color, Time.deltaTime);
            list_Mat[0].SetColor("_SkyTint", c);
            yield return null;
        }
    }
}
