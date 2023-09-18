using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{
    public class FrottageTool : GrabbableEvents
    {
        public CustomRenderTexture rt;
        public Texture2D frottage_Tex;

        Vector3 pos;
        Quaternion rot;

        public override void OnGrab(Grabber grabber)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            base.OnGrab(grabber);
        }

        public override void OnRelease()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.SetPositionAndRotation(pos, rot);
            base.OnRelease();
        }

        // Start is called before the first frame update
        void Start()
        {
            pos = transform.position;
            rot = transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
};