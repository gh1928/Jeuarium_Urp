using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using BNG;

public class ANM_GoghYellowhouse_Tool : GrabbableEvents
{

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public override void OnGrab(Grabber _grabber)
    {
        base.OnGrab(_grabber);

        GetComponent<Rigidbody>().isKinematic = false;
    }

    public override void OnRelease()
    {
        base.OnRelease();

        GetComponent<Rigidbody>().isKinematic = true;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    ////////// Unity            //////////
    // Start is called before the first frame update
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}
