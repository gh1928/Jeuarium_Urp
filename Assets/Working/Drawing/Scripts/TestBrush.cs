using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for Test
public class TestBrush : BNG.GrabbableEvents
{
    public Material mat;
    public Transform pivot;
    public RenderTexture rt;

    Texture2D tex;
    RaycastHit rhit;

    public override void OnGrab(Grabber grabber)
    {
        GetComponent<Rigidbody>().isKinematic = false;

        base.OnGrab(grabber);
    }

    public override void OnRelease()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        base.OnRelease();
    }

    private void Start()
    {
        tex = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
        //mat.SetTexture("_MainTex", tex);
        mat.SetFloat("_Raycast_Bool", 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Canvas"))
        {

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Canvas"))
        {
            if (Physics.Raycast(new Ray(pivot.position, pivot.up), out rhit, 0.1f, 1 << LayerMask.NameToLayer("Canvas")))
            {
                mat.SetFloat("_Raycast_Bool", 1.0f);
                mat.SetVector("_Raycast_UV", rhit.textureCoord);
                Graphics.Blit(tex, rt, mat);
            }
            else
            {
                mat.SetFloat("_Raycast_Bool", 0.0f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Canvas"))
        {
            mat.SetFloat("_Raycast_Bool", 0.0f);
        }
    }
}
