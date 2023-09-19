using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterEvtHandler : MonoBehaviour
{
    public DoorController door;

    //이벤트 시간 제어 변수
    public float[] eventWaitTime;

    public LetterController letterController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartCoroutine(LetterFlyEventCoroutine());
    }

    IEnumerator LetterFlyEventCoroutine()
    {
        //문 열기
        door.TryMoveDoor(50f, 1f);
        
        yield return new WaitForSeconds(eventWaitTime[0]);

        //편지 활성화
        letterController.EnableLetter();
        
        yield return new WaitForSeconds(eventWaitTime[1]);

        //편지 이동 시작
        letterController.StartLetterRotation();

        //문 닫기
        door.TryMoveDoor(0f, 1f, false);

        yield break;
    }
}
