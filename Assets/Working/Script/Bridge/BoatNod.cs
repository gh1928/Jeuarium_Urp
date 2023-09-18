using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandSide
{
    Left = -1,
    Right = 1
};

public class BoatNod : MonoBehaviour
{
    public BoatRide boat;
    public HandSide side;
    public Transform hand;

    AudioSource audioSource;
    Coroutine cor;
    float deltaZ;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        transform.LookAt(hand, -Vector3.up);
        if (!hand.GetComponent<BoatNodGrab>().isGrab && cor != null)
        {
            StopCoroutine(cor);
            cor = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Liquid") && hand.GetComponent<BoatNodGrab>().isGrab)
        {
            if (cor == null)
            {
                audioSource.Play();
                deltaZ = hand.localPosition.z;
                cor = StartCoroutine(RotatingBoat());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Liquid") && cor != null)
        {
            StopCoroutine(cor);
            cor = null;
        }
    }

    IEnumerator RotatingBoat()
    {
        while (true)
        {
            yield return new WaitUntil(() => { return hand.hasChanged; });

            boat.angleY -= (hand.localPosition.z - deltaZ) * (float)side * 10.0f;
            deltaZ = hand.localPosition.z;
            hand.hasChanged = false;
        }
    }
}
