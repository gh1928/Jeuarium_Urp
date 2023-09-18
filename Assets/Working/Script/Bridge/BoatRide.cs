using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatRide : MonoBehaviour
{
    public static BoatRide instance;

    [HideInInspector]
    public float angleY = 90.0f;

    public Transform startline;
    public Transform endline;

    float height;
    Coroutine cor;
    Transform origin_parent;
    Transform boat;

    private void Start()
    {
        height = GetComponent<BNG.BNGPlayerController>().CharacterControllerYOffset;
        origin_parent = transform.parent;
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Boat"))
        {
            boat = hit.transform.parent;
            if (cor == null)
                cor = StartCoroutine(RideBoat());
        }
    }

    IEnumerator RideBoat()
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<BNG.BNGPlayerController>().CharacterControllerYOffset = -0.8f;
        boat.GetComponent<BuoyancyEffecter>().isRide = true;
        transform.parent = boat;
        transform.localPosition = new Vector3(0, transform.localPosition.y, -3f);
        boat.position = startline.position;
        while (true)
        {
            boat.rotation = Quaternion.Lerp(boat.rotation, Quaternion.Euler(0, angleY, 0), Time.deltaTime);
            yield return null;
        }
    }

    public void StopRide()
    {
        StopCoroutine(cor);
        cor = null;

        GetComponent<CharacterController>().enabled = true;
        GetComponent<BNG.BNGPlayerController>().CharacterControllerYOffset = height;
        transform.parent = origin_parent;
        transform.position = endline.position;
        boat.GetComponent<BuoyancyEffecter>().isRide = false;
    }
}
