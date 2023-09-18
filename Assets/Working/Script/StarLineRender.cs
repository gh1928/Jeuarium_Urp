using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

public class StarLineRender : BNG.GrabbableEvents
{
    public LineRenderer line;
    public AudioClip star1;
    public AudioClip star2;
    Vector3 origin;
    Coroutine cor;
    ParticleSystem particle;
    AudioSource audioSource;
    Grabber g;
    Queue<Collider> queue_Cols = new Queue<Collider>();

    TriggerSet trigger;

    public List<Collider> cols;
    public GameObject next_Star;
    public UnityEvent events;

    bool isDone = false;

    public override void OnGrab(Grabber grabber)
    {
        g = grabber;

        GetComponent<Rigidbody>().isKinematic = false;
        if (!particle.isPlaying) particle.Play();
        cor = StartCoroutine(StartLine());

        base.OnGrab(grabber);
    }

    public override void OnRelease()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        StopCoroutine(cor);

        if (isDone)
        {
            line.positionCount--;
            transform.position = origin;
            trigger.SwitchOn();
            if (events != null) events.Invoke();
        }
        else
        {
            // 황영재 수정.
            // 대표님 요청에 따라 중간에 진행이 가능하도록 수정.
            int number = cols.Count - queue_Cols.Count - 2;
            Debug.Log(number);
            switch(number)
            {
                case -1:    { transform.position = origin;                          }   break;
                default:    { transform.position = cols[number].transform.position; }   break;
            }

            line.positionCount = number + 2;

            // 기존 소스. 처음으로 초기화합니다.
            {
                //line.positionCount = 1;
                //transform.position = origin;
                //queue_Cols.Clear();
                //
                //foreach (Collider c in cols)
                //{
                //    if (c.TryGetComponent(out particle)) particle.Stop();
                //    c.enabled = true;
                //    c.transform.GetChild(0).gameObject.SetActive(false);
                //    queue_Cols.Enqueue(c);
                //}
                //particle = transform.parent.GetComponent<ParticleSystem>();
                //particle.Play();
                //queue_Cols.Dequeue().enabled = true;
            }
        }
        
        base.OnRelease();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = cols[0].transform;
        source.weight = 1.0f;
        transform.parent.GetComponent<LookAtConstraint>().SetSource(0, source);

        foreach (Collider c in cols)
        {
            queue_Cols.Enqueue(c);
        }

        particle = transform.parent.GetComponent<ParticleSystem>();
        particle.Play();
        origin = transform.position;

        line.SetPosition(0, Vector3.zero);
        queue_Cols.Dequeue().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            particle.Stop();
            if (other.TryGetComponent(out particle)) particle.Play();

            line.positionCount++;
            line.SetPosition(line.positionCount - 2, other.transform.position - origin);

            if (queue_Cols.Count > 0)
            {
                audioSource.clip = star1;
                audioSource.Play();
                queue_Cols.Dequeue().enabled = true;
            }
            else
            {
                audioSource.clip = star2;
                audioSource.Play();
                isDone = true;
                if (next_Star != null)
                {
                    next_Star.GetComponent<Collider>().enabled = true;
                    next_Star.GetComponent<StarLineRender>().enabled = true;
                    next_Star.GetComponent<Grabbable>().enabled = true;
                }

                GetComponent<Grabbable>().DropItem(g);
            }

            other.transform.GetChild(0).gameObject.SetActive(true);
            other.GetComponent<Collider>().enabled = false;
        }
    }

    IEnumerator StartLine()
    {
        line.positionCount++;

        while (true)
        {
            line.SetPosition(line.positionCount - 1, transform.position - origin);
            yield return null;
        }
    }

    public void SetTrigger(TriggerSet t)
    {
        t.AddTrigger();
        trigger = t;
    }
}
