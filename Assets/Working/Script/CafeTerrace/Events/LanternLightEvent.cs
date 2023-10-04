using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternLightEvent : MonoBehaviour, Ievent
{
    public GameObject lightTrail;
    public Transform target;

    public void OnEvent()
    {
        StartCoroutine(LightOnCoroutine());
    }

    private IEnumerator LightOnCoroutine()
    {
        var trail = Instantiate(lightTrail);

        yield break;
    }
}
