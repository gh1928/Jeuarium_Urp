using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    public static BridgeController instance;

    public Transform bridge_Left;
    public Transform bridge_Right;
    public Transform bottom_Left;
    public Transform bottom_Right;
    public float speed = 1.0f;

    Coroutine cor = null;
    public bool isOpen = false;
    float sum = 0;

    private void Start()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            BridgeChange();
        }
    }

    public void BridgeChange()
    {
        isOpen = !isOpen;

        if (cor != null)
        {
            sum = 50.0f - sum;
        }
        else cor = StartCoroutine(BridgeControl());
    }

    IEnumerator BridgeControl()
    {
        while (sum < 50.0f)
        {
            if (isOpen)
            {
                bridge_Left.Rotate(new Vector3(Time.fixedDeltaTime * speed, 0, 0));
                bottom_Left.Rotate(new Vector3(Time.fixedDeltaTime * speed, 0, 0));
                bridge_Right.Rotate(new Vector3(-Time.fixedDeltaTime * speed, 0, 0));
                bottom_Right.Rotate(new Vector3(-Time.fixedDeltaTime * speed, 0, 0));
            }
            else
            {
                bridge_Left.Rotate(new Vector3(-Time.fixedDeltaTime * speed, 0, 0));
                bottom_Left.Rotate(new Vector3(-Time.fixedDeltaTime * speed, 0, 0));
                bridge_Right.Rotate(new Vector3(Time.fixedDeltaTime * speed, 0, 0));
                bottom_Right.Rotate(new Vector3(Time.fixedDeltaTime * speed, 0, 0));
            }

            sum += Time.fixedDeltaTime * speed;

            yield return null;
        }
        sum = 0;
        cor = null;
        yield break;
    }
}
