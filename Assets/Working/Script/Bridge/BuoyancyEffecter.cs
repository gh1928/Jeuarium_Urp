using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuoyancyEffecter : MonoBehaviour
{
    Rigidbody rb;
    float height;

    public Transform startline;

    public InputActionAsset _inputActionAsset;
    public InputActionReference _horizontalAxis;

    public float speed = 0.2f;
    [HideInInspector]
    public int count = 0;
    [HideInInspector]
    public bool isRide = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        height = transform.position.y;
    }

    private void FixedUpdate()
    {
        rb.AddForce((count + 0.99f + (Mathf.Sin((height - transform.position.y) * Mathf.PI * speed * 10.0f) * (height - transform.position.y))) * -Physics.gravity, ForceMode.Acceleration);

        if (isRide)
        {
            Vector3 relV = rb.transform.InverseTransformDirection(rb.velocity);
            relV.x = _horizontalAxis.action.ReadValue<Vector2>().x;
            relV.z = speed;
            rb.velocity = rb.transform.TransformDirection(relV);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            if (BridgeController.instance.isOpen)
                BridgeController.instance.BridgeChange();
            transform.eulerAngles = new Vector3(0, 90, 0);
            BoatRide.instance.angleY = 90.0f;
            transform.position = startline.position;
        }
        else if (collision.collider.CompareTag("Finish"))
        {
            BoatRide.instance.StopRide();
        }
    }
}
