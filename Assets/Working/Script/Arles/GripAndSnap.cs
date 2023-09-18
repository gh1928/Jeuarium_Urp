using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripAndSnap : BNG.GrabbableEvents
{
    public GameObject snapzone;

    AudioSource audioSource;
    TriggerSet trigger;
    bool isOK = false;
    bool isGrab = false;

    protected override void Awake()
    {
        grab = GetComponent<BNG.Grabbable>();
        input = BNG.InputBridge.Instance;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public override void OnGrab(BNG.Grabber grabber)
    {
        isGrab = true;
        GetComponent<Rigidbody>().isKinematic = false;
        base.OnGrab(grabber);
    }

    public override void OnRelease()
    {
        isGrab = false;
        GetComponent<Rigidbody>().isKinematic = true;
        base.OnRelease();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == snapzone && !isOK)
        {
            transform.parent = snapzone.transform;
            StartCoroutine(PositionLock());
            isOK = true;
        }
    }

    public void SetTrigger(TriggerSet t)
    {
        t.AddTrigger();
        trigger = t;
    }

    /// <summary>
    /// Ư�� ������ �浹 �� �۵��Ǵ� �ڷ�ƾ�Դϴ�. ������Ʈ�� �ش� ��ġ�� ������ŵ�ϴ�. ������ ���¿��� ���� ���� Grabbable ������Ʈ�� ��Ȱ��ȭ�˴ϴ�.
    /// </summary>
    /// <returns></returns>
    IEnumerator PositionLock()
    {
        while (isGrab)
        {
            transform.SetPositionAndRotation(snapzone.transform.position, snapzone.transform.rotation);
            yield return null;
        }
        audioSource.Play();
        transform.SetPositionAndRotation(snapzone.transform.position, snapzone.transform.rotation);
        GetComponent<BNG.Grabbable>().enabled = false;
        trigger.SwitchOn();
        yield return null;
    }
}