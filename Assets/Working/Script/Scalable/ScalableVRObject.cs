using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class ScalableVRObject : MonoBehaviour
{
    public Camera internalCam;

    private Grabbable grabbable;

    private int scaleProceed;

    private float origSize;
    private float origDistance;
    private float origFov;

    Transform rightHand;
    Transform leftHand;

    // Start is called before the first frame update
    void Start()
    {
        grabbable = GetComponent<Grabbable>();
        leftHand = GameObject.Find("LeftControllerAnchor").transform;
        rightHand = GameObject.Find("RightControllerAnchor").transform;
        origFov = internalCam.orthographicSize/transform.localScale.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (grabbable.BeingHeldWithTwoHands)
        {
            Vector3 handPos1 = leftHand.position;// grabbable.GetGrabberVector3(grabbable.HeldByGrabbers[0], false);
            Vector3 handPos2 = rightHand.position;//grabbable.GetGrabberVector3(grabbable.HeldByGrabbers[1], true);

            Debug.DrawLine(handPos1, handPos2, Color.red, 1f);

            switch (scaleProceed)
            {
                //case 0:
                //    scaleProceed = 1;
                //    return;
                case 10:
                    origSize = transform.localScale.x;
                    Debug.Log(origSize);
                    origDistance = Vector3.Distance(handPos1, handPos2);
                    Debug.Log(origDistance);
                    scaleProceed = 12;
                    return;
                case 12:
                    transform.localScale = Vector3.one * origSize * (Vector3.Distance(handPos1, handPos2) / Mathf.Max(origDistance, .0001f));
                    internalCam.orthographicSize = transform.localScale.x * origFov;
                    return;
                default:
                    scaleProceed++;
                    return;
            }


            
            
        }

        scaleProceed = 0;
    }
}
