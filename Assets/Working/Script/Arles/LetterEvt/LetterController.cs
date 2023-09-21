using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
public partial class LetterController : GrabbableEvents
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

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public void EnableLetter()
    {
        letter.SetActive(true);
    }

    //
    public override void OnGrab(Grabber grabber)
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        base.OnGrab(grabber);
    }

    public void StartLetterRotation()
    {
        rotateCoroutine = StartCoroutine(LetterRotateCoroutine());
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

            // Ȳ���� �߰�. ���ڸ� ������ �� �����ļ� �߰��߽��ϴ�.
            TrailRenderer tr = letter.GetComponent<TrailRenderer>();
            if (tr.enabled)
            {
                tr.time -= Time.deltaTime;
                if (tr.time <= 0.0f)
                {
                    tr.time = 0.0f;
                    tr.enabled = false;
                }
            }
        }
    }

    ////////// Unity            //////////

    protected override void Awake()
    {
        base.Awake();
        grabbable = GetComponent<Grabbable>();
    }

    protected void Update()
    {
        ANM_HC_Update();
    }
}

// Ȳ���� �߰�.
partial class LetterController
{
    // �տ� �������� üũ�ϰ� �������� �Ѿ�ô�.
    [Header("HAND_CONTROLLERS ==================================================")]
    [SerializeField] List<HandController> ANM_HC_controllers;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////

    ////////// Unity            //////////
    void ANM_HC_Update()
    {
        for(int i = 0; i < ANM_HC_controllers.Count; i++)
        {
            if((ANM_HC_controllers[i].PreviousHeldObject != null) && (ANM_HC_controllers[i].PreviousHeldObject.Equals(this.gameObject)))
            {
                ANM_HC_controllers[i].grabber.TryRelease();
                this.gameObject.SetActive(false);
            }
        }
    }
}