using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintMarbling : BNG.GrabbableEvents
{
    RaycastHit rHit;
    Texture2D tex;

    MeshRenderer mr;

    public override void OnGrab(Grabber grabber)
    {
        GetComponent<Rigidbody>().isKinematic = false;

        base.OnGrab(grabber);
    }

    public override void OnRelease()
    {
        base.OnRelease();

        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void Start()
    {
        tex = new Texture2D(1024, 1024, TextureFormat.RGBA32, true);
        mr = GetComponent<MeshRenderer>();
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(new Ray(transform.position, -transform.forward), out rHit, 0.1f, 1 << LayerMask.NameToLayer("Canvas")))
        {
            Graphics.CopyTexture(rHit.transform.GetComponent<MeshRenderer>().material.GetTexture("_MainTex"), tex);
            mr.material.SetTexture("_MainTex", tex);
        }
    }
}
