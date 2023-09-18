using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRotation : MonoBehaviour
{
    int count = 0;
    bool isStop = false;

    public void ToBrush()
    {
        if (count != 1)
        {
            isStop = true;
            count = 1;
            StartCoroutine(ClockRotation());
        }
    }

    public void ToColor()
    {
        if (count != 0)
        {
            isStop = false;
            count = 0;
            StartCoroutine(CounterClockRotation());
        }
    }

    IEnumerator ClockRotation()
    {
        while (true)
        {
            if (transform.localEulerAngles.y >= 29.99f || !isStop) break;
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0, 30, 0), 3 * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }

    IEnumerator CounterClockRotation()
    {
        while (true)
        {
            if (transform.localEulerAngles.y <= 0.01f || isStop) break;
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0, 0, 0), 3 * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
}
