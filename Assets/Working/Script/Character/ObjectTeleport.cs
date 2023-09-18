using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTeleport : MonoBehaviour
{
    public GameObject toPosition;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = toPosition.transform.position;
    }
}
