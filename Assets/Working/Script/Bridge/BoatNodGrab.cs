using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatNodGrab : BNG.GrabbableEvents
{
    [HideInInspector]
    public bool isGrab = false;

    public Transform offset;
    public float radius = 1.5f;

    Vector3 pos;
    Quaternion rot;

    public override void OnGrab(Grabber grabber)
    {
        GetComponent<Rigidbody>().isKinematic = false;
        isGrab = true;

        base.OnGrab(grabber);
    }

    public override void OnRelease()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        isGrab = false;
        transform.localPosition = pos;
        transform.localRotation = rot;

        base.OnRelease();
    }

    void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        pos = transform.localPosition;
        rot = transform.localRotation;
    }
}
