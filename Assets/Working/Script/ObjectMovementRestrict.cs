using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{
    public class ObjectMovementRestrict : GrabbableEvents
    {
        public bool move_Position = false;
        public Vector3 dest_Pos;

        public bool move_Rotation = false;
        public bool isFree = false;
        public bool isClock = true;
        public Vector3 dest_Rot;

        public bool move_Scale = false;
        public Vector3 dest_Scale;

        public bool isClamp = false;
        public Vector3 min_Clamp;
        public Vector3 max_Clamp;

        Vector3 pos;
        Vector3 scale;
        Quaternion rot;

        float dist = 0.0f;

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
            pos = transform.position;
            rot = transform.localRotation;
            scale = transform.localScale;

            dist = (dest_Pos - pos).magnitude;
        }

        void Update()
        {
            if (isClamp)
            {
                transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, min_Clamp.x, max_Clamp.x), Mathf.Clamp(transform.localPosition.y, min_Clamp.y, max_Clamp.y), Mathf.Clamp(transform.localPosition.z, min_Clamp.z, max_Clamp.z));
            }

            if (move_Scale)
            {
                transform.localScale = new Vector3(scale.x, scale.y, (1 + ((transform.position - pos).magnitude / dist)) * scale.z);
            }
            else transform.localScale = scale;

            if (move_Rotation)
            {
                if (isClock && !isFree)
                    transform.localEulerAngles = new Vector3(Mathf.Clamp(transform.localEulerAngles.x, rot.eulerAngles.x, dest_Rot.x), Mathf.Clamp(transform.localEulerAngles.y, rot.eulerAngles.y, dest_Rot.y), Mathf.Clamp(transform.localEulerAngles.z, rot.eulerAngles.z, dest_Rot.z));
                else if (!isClock && !isFree)
                    transform.localEulerAngles = new Vector3(Mathf.Clamp(transform.localEulerAngles.x, dest_Rot.x, rot.eulerAngles.x), Mathf.Clamp(transform.localEulerAngles.y, dest_Rot.y, rot.eulerAngles.y), Mathf.Clamp(transform.localEulerAngles.z, dest_Rot.z, rot.eulerAngles.z));
            }
            else transform.localRotation = rot;

            if (move_Position)
            {

            }
            else transform.position = pos;
        }

        public void TestLog()
        {
            Debug.Log("OK");
        }
    }
};