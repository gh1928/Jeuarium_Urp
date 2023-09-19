using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
public class LetterController : GrabbableEvents
{
    //�����̵��� �����ϴ� Ŭ����. 
    
    //������ �����׸��� �Ʒ��� �̵���

    //�� �̺�Ʈ �ð�
    public float totalTime;

    //ȸ�� �ӵ� ���� ����
    public float ControllerMaxRotSpeed;
    public float ControllerMinRotSpeed;

    //�󸶳� �Ʒ��� �������� ���ϴ� ����
    public float ControllerDescentDistance;

    //���� �̵��� ���� �� ���� ��ü ȸ�� �ӵ�
    public float letterRotSpeed;

    public GameObject letter;

    public RingHelper ringHelper;

    private Grabbable grabbable;

    private Coroutine rotateCoroutine;

    protected override void Awake()
    {
        base.Awake();
        grabbable = GetComponent<Grabbable>();
    }
    public void EnableLetter()
    {
        letter.SetActive(true);
    }
    public void StartLetterRotation()
    {
        rotateCoroutine = StartCoroutine(LetterRotateCoroutine());
    }

    public override void OnGrab(Grabber grabber)
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);        

        base.OnGrab(grabber);
    }
    IEnumerator LetterRotateCoroutine()
    {
        float timer = 0;
        float inverseTotalTime = 1 / totalTime;

        float currHeight = transform.position.y;
        float targetHeigth = transform.position.y - ControllerDescentDistance;
        Vector3 newControllerPos = transform.position;

        Transform letterTransform = letter.transform;
        Vector3 newLetterPos = Vector3.zero;
        float startRadius = letterTransform.localPosition.z;

        while (timer < totalTime)
        {
            
            timer += Time.deltaTime;

            //0 ~ 1���� ��
            float timerNormalized = timer * inverseTotalTime;

            //ó���� ������ ȸ���ϴٰ� ������ ������ ȸ��
            float t = Mathf.Sin(timerNormalized * Mathf.PI * 0.5f);
            float rotateSpeed = Mathf.Lerp(ControllerMaxRotSpeed, ControllerMinRotSpeed, t) * Time.deltaTime;
            transform.Rotate(0, rotateSpeed, 0);

            //�� ũ�Ⱑ ���� �پ��
            newLetterPos.z = Mathf.Lerp(startRadius, 0, timerNormalized);
            letterTransform.localPosition = newLetterPos;

            //���� �Ʒ��� ������
            newControllerPos.y = Mathf.Lerp(currHeight, targetHeigth, timerNormalized);
            transform.position = newControllerPos;

            yield return null;
        }

        //�̵��� ������ ������ ���� �� �ֵ�����
        ringHelper.gameObject.SetActive(true);
        grabbable.enabled = true;

        //������ �յ� ���ٴϸ� ���ڸ����� ȸ��
        while (true)
        {
            transform.Translate(0.05f * Time.deltaTime * Mathf.Cos(Time.time) * Vector3.up);
            transform.Rotate(0, letterRotSpeed * Time.deltaTime, 0);
            yield return null;
        }
    }
}
