using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TimeAttack : MonoBehaviour
{
    public TextMeshProUGUI minute;
    public TextMeshProUGUI second;
    public UnityEvent _event;

    float m = 0;
    float s = 0;
    Coroutine cor;

    void Start()
    {
        m = float.Parse(minute.text);
        s = float.Parse(second.text);
        StartTimer();
    }

    public void StartTimer()
    {
        cor = StartCoroutine(Timer());
    }

    public void EndTimer()
    {
        StopCoroutine(cor);
        gameObject.SetActive(false);
    }

    IEnumerator Timer()
    {
        while (true)
        {
            s -= Time.deltaTime;
            if (s < 0.0f)
            {
                if (m > 0)
                {
                    m -= 1.0f;
                    s += 60.0f;
                }
                else
                {
                    s = 0;
                    break;
                }
            }

            minute.text = ((int)m).ToString("D2");
            second.text = ((int)s).ToString("D2");

            yield return null;
        }

        _event.Invoke();
    }
}
