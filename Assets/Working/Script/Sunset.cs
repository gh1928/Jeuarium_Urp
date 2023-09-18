using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunset : MonoBehaviour
{
    public Transform sun;
    public Material sky;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(SunMove());
            GetComponent<Collider>().enabled = false;
        }
    }

    IEnumerator SunMove()
    {
        while (sun.eulerAngles.x != 0.0f)
        {
            sun.eulerAngles = Vector3.Lerp(sun.eulerAngles, new Vector3(0.0f, sun.eulerAngles.y, sun.eulerAngles.z), Time.deltaTime);
            sky.SetColor("_SkyTint", Color.Lerp(sky.GetColor("_SkyTint"), new Color(0.9f, 0.0f, 1.0f, 1.0f), Time.deltaTime));
            yield return null;
        }
    }
}
