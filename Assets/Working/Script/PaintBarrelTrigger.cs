using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PaintBarrelTrigger : BNG.GrabbableEvents
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
        if (other.CompareTag("Canvas"))
        {
            ANM_Brush_Painting();
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

// 황영재 추가
partial class PaintBarrelTrigger
{
    ////////// Getter & Setter  //////////
    public bool ANM_Basic_isDone    { get { return isDone;  }   }

    ////////// Method           //////////

    ////////// Unity            //////////
}

partial class PaintBarrelTrigger
{
    [Header("BRUSH ==================================================")]
    [SerializeField] ANM_Manager Brush_manager;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    // 캔버스에 칠하는 작업을 외부에서도 접근할 수 있게 OnTriggerEnter에서 분리
    public void ANM_Brush_Painting()
    {
        if (!isDone)
        {
            Brush_manager.ANM_Event_Trigger(this.gameObject);
            //trigger.SwitchOn();
            if (GetComponent<AudioSource>() != null)
            {
                GetComponent<AudioSource>().Play();
            }

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

    ////////// Unity            //////////
}