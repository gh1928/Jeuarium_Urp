using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private AudioSource audioSource;

    public Transform doorPivot;

    public Coroutine doorMoveCoroutine;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void TryMoveDoor(float destAngle, float totalTime, bool playSound = true)
    {
        if (doorMoveCoroutine != null)
            StopCoroutine(doorMoveCoroutine);

        doorMoveCoroutine = StartCoroutine(DoorMoveCoroutine(destAngle, totalTime));

        if (playSound)
            audioSource.Play();
    }

    //목표 각도까지 일정시간동안 문을 회전시킴. 문 회전방향도 코드로 정할 수 있게 수정 필요.
    private IEnumerator DoorMoveCoroutine(float destAngle, float totalTime)
    {
        float startAngle = transform.localEulerAngles.y;
        float timer = 0f;

        float invTotalTime = 1 / totalTime;

        Vector3 newAngle = Vector3.zero;

        while (timer < totalTime)
        {
            timer += Time.deltaTime;
            float t = Mathf.Sin(timer * invTotalTime * Mathf.PI * 0.5f);
            newAngle.y = Mathf.Lerp(startAngle, destAngle, t);
            transform.localEulerAngles = newAngle;

            yield return null;
        }

        yield break;
    }
}
