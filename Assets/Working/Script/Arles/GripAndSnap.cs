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
    /// 특정 지점에 충돌 시 작동되는 코루틴입니다. 오브젝트를 해당 위치에 고정시킵니다. 부착된 상태에서 손을 때면 Grabbable 컴포넌트가 비활성화됩니다.
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