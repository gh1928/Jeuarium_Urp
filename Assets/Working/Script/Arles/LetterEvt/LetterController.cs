using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
public partial class LetterController : GrabbableEvents
{
    //편지이동을 제어하는 클래스. 
    
    //편지는 원을그리며 아래로 이동함

    //총 이벤트 시간
    public float totalTime;

    //회전 속도 관련 변수
    public float ControllerMaxRotSpeed;
    public float ControllerMinRotSpeed;

    //얼마나 아래로 내려갈지 정하는 변수
    public float ControllerDescentDistance;

    //편지 이동이 끝난 후 편지 자체 회전 속도
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

            //0 ~ 1사이 값
            float timerNormalized = timer * inverseTotalTime;

            //처음엔 빠르게 회전하다가 갈수록 느리게 회전
            float t = Mathf.Sin(timerNormalized * Mathf.PI * 0.5f);
            float rotateSpeed = Mathf.Lerp(ControllerMaxRotSpeed, ControllerMinRotSpeed, t) * Time.deltaTime;
            transform.Rotate(0, rotateSpeed, 0);

            //원 크기가 점점 줄어듬
            newLetterPos.z = Mathf.Lerp(startRadius, 0, timerNormalized);
            letterTransform.localPosition = newLetterPos;

            //점점 아래로 내려감
            newControllerPos.y = Mathf.Lerp(currHeight, targetHeigth, timerNormalized);
            transform.position = newControllerPos;

            yield return null;
        }

        //이동이 끝나면 편지를 잡을 수 있도록함
        ringHelper.gameObject.SetActive(true);
        grabbable.enabled = true;

        //편지가 둥둥 떠다니며 제자리에서 회전
        while (true)
        {
            transform.Translate(0.05f * Time.deltaTime * Mathf.Cos(Time.time) * Vector3.up);
            transform.Rotate(0, letterRotSpeed * Time.deltaTime, 0);
            yield return null;

            // 황영재 추가. 재자리 돌기할 때 눈아파서 추가했습니다.
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

// 황영재 추가.
partial class LetterController
{
    // 손에 잡혔는지 체크하고 다음으로 넘어갑시다.
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