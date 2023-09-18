using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReflect : MonoBehaviour
{
    Vector3 reflect = Vector3.zero;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            Vector3 income = collision.gameObject.GetComponent<BallScore>().pre_Velocity;
            reflect = income - (2.0f * Vector3.Dot(-income, -transform.forward) * transform.forward);
            collision.gameObject.GetComponent<Rigidbody>().velocity = reflect;
        }
    }
}
