using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    private Transform transform;
    void Start()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        transform = GetComponent<Transform>();

        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            transform.Translate( new Vector3(0, 0, -1.0f * Time.deltaTime));
        }
    }


}
