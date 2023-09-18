using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTest : MonoBehaviour
{
    Rigidbody body;

    public float torque;

    public float force;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();

        body.AddTorque(Vector3.forward * torque);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            body.AddForce(Vector3.right * force);
        }
    }
}
