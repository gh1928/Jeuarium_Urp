using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public enum StageMode { Static = 0, Linear, Circle, Rotate }

    public Transform pivot;
    public StageMode mode = StageMode.Static;
    public float speed = 1.0f;
    public bool isClock = true;

    Vector3 dir;
    Vector3 initPos;

    void Start()
    {
        initPos = transform.position;
        dir = (pivot.position - initPos).normalized;

        switch (mode)
        {
            case StageMode.Static:
                break;
            case StageMode.Linear:
                StartCoroutine(LinearMove());
                break;
            case StageMode.Circle:
                StartCoroutine(CircleMove());
                break;
            case StageMode.Rotate:
                StartCoroutine(RotateMove());
                break;
        }
    }

    IEnumerator LinearMove()
    {
        bool isReverse = false;
        while (true)
        {
            if (BallShooting.instance != null) CheckDone();

            if (!isReverse && Vector3.Distance(transform.position, pivot.position) < 0.01f)
            {
                isReverse = true;
                dir = -dir;
            }

            else if (isReverse && Vector3.Distance(transform.position, initPos) < 0.01f)
            {
                isReverse = false;
                dir = -dir;
            }

            transform.Translate(Time.deltaTime * speed * dir, Space.World);
            yield return null;
        }
    }

    IEnumerator CircleMove()
    {
        while (true)
        {
            if (BallShooting.instance != null) CheckDone();

            if (isClock)
            {
                transform.RotateAround(pivot.position, pivot.up, Time.fixedDeltaTime * Mathf.PI * speed);
            }

            else
            {
                transform.RotateAround(pivot.position, pivot.up, -Time.fixedDeltaTime * Mathf.PI * speed);
            }

            yield return null;
        }
    }

    IEnumerator RotateMove()
    {
        while (true)
        {
            if (BallShooting.instance != null) CheckDone();

            if (isClock)
            {
                transform.Rotate(Time.fixedDeltaTime * Mathf.PI * speed * transform.up);
            }

            else
            {
                transform.Rotate(-Time.fixedDeltaTime * Mathf.PI * speed * transform.up);
            }

            yield return null;
        }
    }

    void CheckDone()
    {
        if (BallShooting.instance.isDone) StopAllCoroutines();
    }
}
