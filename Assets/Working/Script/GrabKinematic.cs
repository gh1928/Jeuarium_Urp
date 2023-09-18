using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    public class GrabKinematic : GrabbableEvents
    {
        private void Start()
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }

        public override void OnGrab(Grabber grabber)
        {
            base.OnGrab(grabber);

            GetComponent<Rigidbody>().isKinematic = false;
        }

        public override void OnRelease()
        {
            base.OnRelease();

            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
};
