using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterEvtHandler : MonoBehaviour
{
    public DoorController door;

    //�̺�Ʈ �ð� ���� ����
    public float[] eventWaitTime;

    public LetterController letterController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartCoroutine(LetterFlyEventCoroutine());
    }

    IEnumerator LetterFlyEventCoroutine()
    {
        //�� ����
        door.TryMoveDoor(50f, 1f);
        
        yield return new WaitForSeconds(eventWaitTime[0]);

        //���� Ȱ��ȭ
        letterController.EnableLetter();
        
        yield return new WaitForSeconds(eventWaitTime[1]);

        //���� �̵� ����
        letterController.StartLetterRotation();

        //�� �ݱ�
        door.TryMoveDoor(0f, 1f, false);

        yield break;
    }
}
