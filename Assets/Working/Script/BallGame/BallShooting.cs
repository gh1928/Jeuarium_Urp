using BNG;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallShooting : MonoBehaviour
{
    public static Dictionary<Color, int> color_Counts = new Dictionary<Color, int>();       // 색깔의 종류와 그 수를 세는 Dictionary
    public static Queue<BallSpawn> ballSpawnQueue = new Queue<BallSpawn>();                 // BallSpawn이 초기화되는 순서를 위한 Queue
    public static float score = 0;                                                          // 공을 터뜨린 점수
    public static float spendTime = 0;                                                      // 경과 시간
    public static int throwCount = 0;                                                       // 버블을 던진 횟수
    public static int life = 3;

    public static BallShooting instance;

    public enum TrajectoryType { Linear = 0, Arc, FreeThrow}    // 던지는 방식 종류 - 선형, 포물선, 자유투

    [Tooltip("발사할 공의 프리팹을 지정합니다.")]
    public GameObject prefab_Ball;

    [Tooltip("다음 색을 알려줄 오브젝트를 지정합니다.")]
    public GameObject next_Ball;

    [Tooltip("피버 상태를 알려줄 텍스트 UI를 지정합니다.")]
    public GameObject fever_Text;

    [Tooltip("충돌 지점을 알려줄 오브젝트를 지정합니다.")]
    public GameObject ghostBall;

    [Tooltip("던지는 방식을 정합니다. Linear - 선형, Arc - 포물선, FreeThrow - 자유투")]
    public TrajectoryType trajectoryType;

    [Tooltip("공이 터질 체인 수를 지정합니다.")]
    public int chain = 3;

    [Tooltip("공이 날아갈 힘의 세기를 정합니다.")]
    public float ballForce = 5.0f;

    [Tooltip("피버 상태일 때 공이 날아갈 속력의 배수를 정합니다.")]
    public float feverSpeed = 1.5f;

    [Tooltip("얼마만큼의 체인 배수에 해당하는 점수를 내야 아이템이 사용되는지를 정합니다.")]
    public int item_Limit = 5;

    [Tooltip("얼마만큼의 체인 배수에 해당하는 점수를 내야 피버 상태에 돌입하는지를 정합니다.")]
    public int fever_Limit = 7;

    [Tooltip("조준선의 변곡점 개수를 정합니다.")]
    public int maxIterations = 50;


    public delegate void OnScoreChanged(float currentScore);
    public static OnScoreChanged scoreChanged;

    public List<GameObject> items;

    List<Color> colors;                                         // 스폰된 공의 색 리스트
    Collider[] cols = new Collider[10];
    Transform ball = null;
    Transform temp_Ball = null;
    LineRenderer line;
    Color nextColor;
    Color tempColor;

    [HideInInspector]
    public bool isDone = false;
    bool isLoaded = false;
    bool isShot = false;
    bool isHolding = false;
    bool isFever = false;
    Coroutine cor = null;

    private Vector3 prevPos;
    private Vector3 handVelocity = Vector3.zero;

    InputBridge input;

    void Start()
    {
        instance = this;
        input = InputBridge.Instance;
        line = GetComponent<LineRenderer>();
        scoreChanged += ChangeDisplayedScore;
        StartCoroutine(InitializeList());                       // 색 리스트 초기화
    }

    private void OnDestroy()
    {
        instance = null;
        StopAllCoroutines();
        color_Counts.Clear();
        ballSpawnQueue.Clear();
        score = 0;
        spendTime = 0;
        throwCount = 0;
        life = 3;
        scoreChanged -= ChangeDisplayedScore;
    }

    /// <summary>
    /// 선형 조준선을 그립니다. 충돌 예상 지점을 반환합니다.
    /// </summary>
    /// <returns> Vector3 : 충돌 지점 </returns>
    Vector3 LinearTrajectory()
    {
        line.positionCount = 0;
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 5.0f))
        {
            if (hit.collider.CompareTag("Ball"))
            {
                line.enabled = true;
                line.positionCount = 2;
                line.SetPosition(1, new Vector3(0, 0, hit.distance));
                line.colorGradient.alphaKeys[0].alpha = 150;
                line.colorGradient.alphaKeys[1].alpha = 50;
                return transform.position + transform.forward * (hit.distance - ball.lossyScale.x);
            }
            else if (hit.collider.CompareTag("Canvas"))         // 반사판 충돌 후 예상 지점
            {
                RaycastHit hit2;
                Vector3 reflect = transform.forward - (2.0f * Vector3.Dot(-transform.forward, -hit.transform.forward) * hit.transform.forward);
                if (Physics.Raycast(new Ray(hit.point, reflect), out hit2, 5.0f))
                {
                    if (hit2.collider.CompareTag("Ball"))
                    {
                        line.enabled = true;
                        line.positionCount = 3;
                        line.SetPosition(1, new Vector3(0, 0, hit.distance));
                        line.SetPosition(2, (hit.distance * Vector3.forward) + (hit2.distance * reflect));
                        line.colorGradient.alphaKeys[0].alpha = 150;
                        line.colorGradient.alphaKeys[1].alpha = 50;
                        return transform.position + ((hit.distance - ball.lossyScale.x) * transform.forward) + ((hit2.distance - ball.lossyScale.x) * reflect);
                    }
                }
            }

            else
                line.enabled = false;
        }
        else
            line.enabled = false;

        return Vector3.zero;
    }

    /// <summary>
    /// 포물선형 조준선을 그립니다. 충돌 예상 지점을 반환합니다.
    /// </summary>
    /// <param name="pos">포물선 시작 위치</param>
    /// <param name="forward">포물선 진행 방향</param>
    /// <returns> Vector3 : 충돌 지점 </returns>
    Vector3 ArcTrajectory(Vector3 pos, Vector3 forward)
    {
        line.startColor = Color.white;
        line.endColor = Color.white;
        line.positionCount = 0;
        Vector3 force = new Vector3(forward.x, 0, forward.z);
        Vector3 grav = forward.y * Vector3.up;
        Vector3 next = pos;

        for (int i = 0; i < maxIterations; i++)
        {
            line.positionCount++;
            line.SetPosition(i, next);

            grav += Time.fixedDeltaTime * Physics.gravity;
            next += Time.fixedDeltaTime * force;
            next += Time.fixedDeltaTime * grav;

            if (Physics.OverlapSphereNonAlloc(next, ball.lossyScale.x * 0.5f, cols, 1 << LayerMask.NameToLayer("Canvas")) >= 1)     // 반사판 충돌 후 예상 지점
            {
                Vector3 vel = (next - line.GetPosition(i)) / Time.fixedDeltaTime;
                vel -= (2.0f * Vector3.Dot(-vel, -cols[0].transform.forward) * cols[0].transform.forward);
                force = new Vector3(vel.x, 0, vel.z);

                for (int j = 1; j < maxIterations; j++)
                {
                    line.positionCount++;
                    line.SetPosition(i + j, next);

                    grav += Time.fixedDeltaTime * Physics.gravity;
                    next += Time.fixedDeltaTime * force;
                    next += Time.fixedDeltaTime * grav;

                    if (Physics.OverlapSphereNonAlloc(next, ball.lossyScale.x * 0.5f, cols, 1 << LayerMask.NameToLayer("Ball")) >= 1)
                    {
                        break;
                    }
                }

                break;
            }

            if (Physics.OverlapSphereNonAlloc(next, ball.lossyScale.x * 0.5f, cols, 1 << LayerMask.NameToLayer("Ball")) >= 1)
            {
                break;
            }
        }
        return next;
    }

    /// <summary>
    /// 자유투 방식으로 던지는 벡터를 계산합니다. 트리거를 누를 때 기준점을 정하고, 놓을 때 위치와 기준점 사이의 벡터를 계산합니다.
    /// </summary>
    void FreeThrowing()
    {
        if (isLoaded && !isShot && !isHolding && input.RightTrigger >= 0.8)
        {
            isHolding = true;
            prevPos = transform.position;
        }

        if (isLoaded && !isShot && isHolding && input.RightTrigger >= 0.8)
        {
            ShootingLine();     // 조준선 그리기
        }

        if (isLoaded && !isShot && isHolding && input.RightTrigger < 0.8)
        {
            isHolding = false;
            isShot = true;
            handVelocity = ballForce * (transform.position - prevPos);
            ball.parent = null;
            ball.GetComponent<SphereCollider>().enabled = true;
            Rigidbody ballBody = ball.GetComponent<Rigidbody>();
            ShootBall(ballBody);
            temp_Ball = ball;
            if (cor == null)
                cor = StartCoroutine(TimeOut());

            line.positionCount = 0;
            ghostBall.SetActive(false);
        }
    }

    /// <summary>
    /// 던지는 방식에 따라 조준선을 그립니다.
    /// </summary>
    void ShootingLine()
    {
        float speed = isFever ? feverSpeed : 1;

        if (ball != null)
        {
            switch (trajectoryType)
            {
                case TrajectoryType.Linear:
                    line.useWorldSpace = false;
                    GhostBallPreview(LinearTrajectory());
                    break;
                case TrajectoryType.Arc:
                    line.useWorldSpace = true;
                    GhostBallPreview(ArcTrajectory(transform.position, speed * ballForce * transform.forward));
                    break;
                case TrajectoryType.FreeThrow:
                    line.useWorldSpace = true;
                    GhostBallPreview(ArcTrajectory(transform.position, speed * ballForce * (transform.position - prevPos)));
                    break;
            }
        }
    }

    /// <summary>
    /// 공이 도착할 지점을 미리 표시해줍니다.
    /// </summary>
    /// <param name="pos">반투명 공의 위치</param>
    void GhostBallPreview(Vector3 pos)
    {
        if (pos != Vector3.zero)
        {
            ghostBall.SetActive(true);
            ghostBall.transform.position = pos;
        }

        else ghostBall.SetActive(false);
    }

    /// <summary>
    /// 던지는 방식에 따라 공을 발사합니다.
    /// </summary>
    /// <param name="body"></param>
    void ShootBall(Rigidbody body)
    {
        float speed = isFever ? feverSpeed : 1;
        AddThrowCount();
        body.isKinematic = false;
        switch (trajectoryType)
        {
            case TrajectoryType.Linear:
                body.useGravity = false;
                body.velocity = ball.forward * ballForce * speed;
                break;
            case TrajectoryType.Arc:
                body.useGravity = true;
                body.velocity = ball.forward * ballForce * speed;
                break;
            case TrajectoryType.FreeThrow:
                body.useGravity = true;
                body.velocity = handVelocity * speed;
                break;
        }
    }

    private void AddThrowCount()
    {
        throwCount++;
        AddScore(-10 * (chain - 2));
        if (BallResultScoreBoard.instance != null) BallResultScoreBoard.instance.scoreboard_throwCountUI.text = throwCount.ToString();
    }

    IEnumerator InitializeList()
    {
        while (ballSpawnQueue.Count > 0)
        {
            BallSpawn bs = ballSpawnQueue.Dequeue();
            bs.Initialize();
        }

        colors = color_Counts.Keys.ToList();
        do
            nextColor = colors[Random.Range(0, colors.Count)];
        while (color_Counts[nextColor] == 0);                               // 스테이지에 존재하지 않는 색의 공이 장전되지 않도록 세팅
        StartCoroutine(Shooting());                                         // Shooting() 코루틴 시작
        yield return null;
    }

    /// <summary>
    /// 스테이지의 공을 모두 없앨 때까지 발사 및 장전 시퀀스를 반복합니다.
    /// </summary>
    IEnumerator Shooting()
    {
        int item_Count = 1;
        int fever_Count = 1;
        int remain_Fever = 0;

        while (true)
        {
            //bool confirm = false;             // 각 색상의 공이 얼마나 남아있는지 체크하기 위한 임시변수
            //if (Input.GetKeyDown(KeyCode.P))  // 키보드키 P를 누르면 Debug탭을 통해 각 색상의 남은 개수를 표시함
            //{
            //    confirm = true;
            //}
            isDone = true;
            foreach (int i in color_Counts.Values.ToList())                 // 스테이지에 남은 공이 있는지 체크
            {
                //if(confirm)
                //Debug.Log("Remain ball : " + i);
                if (i > 0)
                    isDone = false;
            }

            if (isDone)
            {
                // 스테이지 클리어 시 작동할 스크립트 내용...
                if (BallResultScoreBoard.instance != null) 
                { BallResultScoreBoard.instance.ActiveBoard(); }   //점수결과창 표시
                Destroy(gameObject);
                break;
            }

            CountingSpendTime();    //경과시간 카운트

            if ((score / fever_Count) >= chain * fever_Limit)
            {
                fever_Count++;
                remain_Fever = fever_Limit / 100;
                isFever = true;
                fever_Text.SetActive(true);
                trajectoryType = TrajectoryType.Linear;
            }

            // 장전 시퀀스
            if (!isLoaded && input.RightTrigger == 0)
            {
                if (ball != null && ball.CompareTag("Ball"))
                {
                    ball.parent.GetComponent<BallSpawn>().AddColorBall(tempColor);      // 발사된 공이 부착되었다면 색깔 카운트 +1
                }

                if ((score / item_Count) >= chain * item_Limit)                         // 일정 점수 이상 달성하면 아이템 장전
                {
                    item_Count++;
                    ball = Instantiate(items[Random.Range(0, items.Count)], transform.position, transform.rotation, transform).transform;
                    ball.tag = "Bullet";
                    ball.gameObject.layer = LayerMask.NameToLayer("Bullet");
                    ball.GetComponent<SphereCollider>().enabled = false;
                    isLoaded = true;
                }
                else
                {
                    while (color_Counts[nextColor] == 0)                                // 스테이지에 존재하지 않는 색의 공이 장전되지 않도록 세팅
                    {
                        nextColor = colors[Random.Range(0, colors.Count)];
                    }
                    ball = Instantiate(prefab_Ball, transform.position, transform.rotation, transform).transform;
                    BallScore bs = ball.GetComponent<BallScore>();
                    bs.collidedThresholdCount = chain;                                  // 체인 수 적용
                    ball.tag = "Bullet";
                    ball.gameObject.layer = LayerMask.NameToLayer("Bullet");
                    ball.GetComponent<SphereCollider>().enabled = false;
                    ball.GetChild(0).GetComponent<MeshRenderer>().material.color = nextColor;
                    ball.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material.color = nextColor;
                    bs.MyColor = nextColor;
                    tempColor = nextColor;
                    do
                        nextColor = colors[Random.Range(0, colors.Count)];
                    while (color_Counts[nextColor] == 0);                               // 다음 공의 색이 스테이지에 존재하는지 체크
                    next_Ball.GetComponent<MeshRenderer>().material.color = nextColor;
                    isLoaded = true;
                }
            }

            if (trajectoryType == TrajectoryType.FreeThrow)
            {
                FreeThrowing();
                yield return null;
                continue;
            }

            ShootingLine();                                                             // 조준선 그리기

            // 발사 시퀀스
            if (input.RightTriggerDown && isLoaded && !isShot)
            {
                isShot = true;
                ball.parent = null;
                ball.GetComponent<SphereCollider>().enabled = true;
                Rigidbody ballBody = ball.GetComponent<Rigidbody>();
                ShootBall(ballBody);

                if (isFever)
                {
                    remain_Fever--;
                    if (remain_Fever == 0)
                    {
                        fever_Text.SetActive(false);
                        isFever = false;
                        trajectoryType = TrajectoryType.Arc;
                    }
                }

                temp_Ball = ball;
                if (cor == null)
                    cor = StartCoroutine(TimeOut());                                    // TimeOut() 코루틴 시작
            }

            yield return null;
        }
        yield return null;
    }

    private void CountingSpendTime()
    {
        spendTime += Time.deltaTime;
        AddScore(-Time.deltaTime * 10.0f * (chain - 2));
    }

    /// <summary>
    /// 공이 부착되지 않고 멀리 날아가면 일정 시간 후에 삭제합니다.
    /// </summary>
    /// <returns></returns>
    IEnumerator TimeOut()
    {
        Coroutine cor2 = StartCoroutine(Reloaded());                                    // Reloaded() 코루틴 시작

        yield return new WaitForSeconds(3.0f);
        if (temp_Ball != null && temp_Ball.CompareTag("Bullet"))
        {
            StopCoroutine(cor2);
            cor2 = null;
            isLoaded = false;
            isShot = false;
            Destroy(temp_Ball.gameObject);
            temp_Ball = null;
            life--;
        }
        cor = null;
        yield return null;
    }

    /// <summary>
    /// 공을 장전해야 하는 상태로 전환합니다.
    /// </summary>
    /// <returns></returns>
    IEnumerator Reloaded()
    {
        yield return new WaitUntil(() => { if (ball != null) return ball.CompareTag("Ball"); else { return true; } });
        StopCoroutine(cor);
        cor = null;
        isLoaded = false;
        isShot = false;
        yield return null;
    }

    public static void AddScore(float value)
    {
        score += value;
        scoreChanged(score);
    }

    public void ChangeDisplayedScore(float currentScore)
    {
        if(BallResultScoreBoard.instance!=null)
            BallResultScoreBoard.instance.scoreboard_scoreUI.text = score.ToString("F0");
    }
}
