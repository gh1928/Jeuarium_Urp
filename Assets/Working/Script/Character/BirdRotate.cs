using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdRotate : MonoBehaviour
{
    private Transform transform;

    void Start()
    {
        StartCoroutine(Rotate());
    }
    IEnumerator Rotate()
    {
        transform = GetComponent<Transform>();

        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            transform.Rotate(new Vector3(0, -Time.deltaTime*20, 0));
        }
    }
}
