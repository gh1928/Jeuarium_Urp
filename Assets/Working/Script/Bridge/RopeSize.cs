using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSize : MonoBehaviour
{
    public Transform endPoint;
    float initDist;

    // Start is called before the first frame update
    void Start()
    {
        initDist = Vector3.Distance(transform.position, endPoint.position);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(endPoint);
        transform.Rotate(new Vector3(90.0f, 0f, 0f));

        float dist = Vector3.Distance(transform.position, endPoint.position);
        transform.localScale = new Vector3(1, dist / initDist, 1);
    }
}
