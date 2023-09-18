using BNG;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallShooting : MonoBehaviour
{
    public static Dictionary<Color, int> color_Counts = new Dictionary<Color, int>();       // ������ ������ �� ���� ���� Dictionary
    public static Queue<BallSpawn> ballSpawnQueue = new Queue<BallSpawn>();                 // BallSpawn�� �ʱ�ȭ�Ǵ� ������ ���� Queue
    public static float score = 0;                                                          // ���� �Ͷ߸� ����
    public static float spendTime = 0;                                                      // ��� �ð�
    public static int throwCount = 0;                                                       // ������ ���� Ƚ��
    public static int life = 3;

    public static BallShooting instance;

    public enum TrajectoryType { Linear = 0, Arc, FreeThrow}    // ������ ��� ���� - ����, ������, ������

    [Tooltip("�߻��� ���� �������� �����մϴ�.")]
    public GameObject prefab_Ball;

    [Tooltip("���� ���� �˷��� ������Ʈ�� �����մϴ�.")]
    public GameObject next_Ball;

    [Tooltip("�ǹ� ���¸� �˷��� �ؽ�Ʈ UI�� �����մϴ�.")]
    public GameObject fever_Text;

    [Tooltip("�浹 ������ �˷��� ������Ʈ�� �����մϴ�.")]
    public GameObject ghostBall;

    [Tooltip("������ ����� ���մϴ�. Linear - ����, Arc - ������, FreeThrow - ������")]
    public TrajectoryType trajectoryType;

    [Tooltip("���� ���� ü�� ���� �����մϴ�.")]
    public int chain = 3;

    [Tooltip("���� ���ư� ���� ���⸦ ���մϴ�.")]
    public float ballForce = 5.0f;

    [Tooltip("�ǹ� ������ �� ���� ���ư� �ӷ��� ����� ���մϴ�.")]
    public float feverSpeed = 1.5f;

    [Tooltip("�󸶸�ŭ�� ü�� ����� �ش��ϴ� ������ ���� �������� ���Ǵ����� ���մϴ�.")]
    public int item_Limit = 5;

    [Tooltip("�󸶸�ŭ�� ü�� ����� �ش��ϴ� ������ ���� �ǹ� ���¿� �����ϴ����� ���մϴ�.")]
    public int fever_Limit = 7;

    [Tooltip("���ؼ��� ������ ������ ���մϴ�.")]
    public int maxIterations = 50;


    public delegate void OnScoreChanged(float currentScore);
    public static OnScoreChanged scoreChanged;

    public List<GameObject> items;

    List<Color> colors;                                         // ������ ���� �� ����Ʈ
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
        StartCoroutine(InitializeList());                       // �� ����Ʈ �ʱ�ȭ
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
    /// ���� ���ؼ��� �׸��ϴ�. �浹 ���� ������ ��ȯ�մϴ�.
    /// </summary>
    /// <returns> Vector3 : �浹 ���� </returns>
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
            else if (hit.collider.CompareTag("Canvas"))         // �ݻ��� �浹 �� ���� ����
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
    /// �������� ���ؼ��� �׸��ϴ�. �浹 ���� ������ ��ȯ�մϴ�.
    /// </summary>
    /// <param name="pos">������ ���� ��ġ</param>
    /// <param name="forward">������ ���� ����</param>
    /// <returns> Vector3 : �浹 ���� </returns>
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

            if (Physics.OverlapSphereNonAlloc(next, ball.lossyScale.x * 0.5f, cols, 1 << LayerMask.NameToLayer("Canvas")) >= 1)     // �ݻ��� �浹 �� ���� ����
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
    /// ������ ������� ������ ���͸� ����մϴ�. Ʈ���Ÿ� ���� �� �������� ���ϰ�, ���� �� ��ġ�� ������ ������ ���͸� ����մϴ�.
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
            ShootingLine();     // ���ؼ� �׸���
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
    /// ������ ��Ŀ� ���� ���ؼ��� �׸��ϴ�.
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
    /// ���� ������ ������ �̸� ǥ�����ݴϴ�.
    /// </summary>
    /// <param name="pos">������ ���� ��ġ</param>
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
    /// ������ ��Ŀ� ���� ���� �߻��մϴ�.
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
        while (color_Counts[nextColor] == 0);                               // ���������� �������� �ʴ� ���� ���� �������� �ʵ��� ����
        StartCoroutine(Shooting());                                         // Shooting() �ڷ�ƾ ����
        yield return null;
    }

    /// <summary>
    /// ���������� ���� ��� ���� ������ �߻� �� ���� �������� �ݺ��մϴ�.
    /// </summary>
    IEnumerator Shooting()
    {
        int item_Count = 1;
        int fever_Count = 1;
        int remain_Fever = 0;

        while (true)
        {
            //bool confirm = false;             // �� ������ ���� �󸶳� �����ִ��� üũ�ϱ� ���� �ӽú���
            //if (Input.GetKeyDown(KeyCode.P))  // Ű����Ű P�� ������ Debug���� ���� �� ������ ���� ������ ǥ����
            //{
            //    confirm = true;
            //}
            isDone = true;
            foreach (int i in color_Counts.Values.ToList())                 // ���������� ���� ���� �ִ��� üũ
            {
                //if(confirm)
                //Debug.Log("Remain ball : " + i);
                if (i > 0)
                    isDone = false;
            }

            if (isDone)
            {
                // �������� Ŭ���� �� �۵��� ��ũ��Ʈ ����...
                if (BallResultScoreBoard.instance != null) 
                { BallResultScoreBoard.instance.ActiveBoard(); }   //�������â ǥ��
                Destroy(gameObject);
                break;
            }

            CountingSpendTime();    //����ð� ī��Ʈ

            if ((score / fever_Count) >= chain * fever_Limit)
            {
                fever_Count++;
                remain_Fever = fever_Limit / 100;
                isFever = true;
                fever_Text.SetActive(true);
                trajectoryType = TrajectoryType.Linear;
            }

            // ���� ������
            if (!isLoaded && input.RightTrigger == 0)
            {
                if (ball != null && ball.CompareTag("Ball"))
                {
                    ball.parent.GetComponent<BallSpawn>().AddColorBall(tempColor);      // �߻�� ���� �����Ǿ��ٸ� ���� ī��Ʈ +1
                }

                if ((score / item_Count) >= chain * item_Limit)                         // ���� ���� �̻� �޼��ϸ� ������ ����
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
                    while (color_Counts[nextColor] == 0)                                // ���������� �������� �ʴ� ���� ���� �������� �ʵ��� ����
                    {
                        nextColor = colors[Random.Range(0, colors.Count)];
                    }
                    ball = Instantiate(prefab_Ball, transform.position, transform.rotation, transform).transform;
                    BallScore bs = ball.GetComponent<BallScore>();
                    bs.collidedThresholdCount = chain;                                  // ü�� �� ����
                    ball.tag = "Bullet";
                    ball.gameObject.layer = LayerMask.NameToLayer("Bullet");
                    ball.GetComponent<SphereCollider>().enabled = false;
                    ball.GetChild(0).GetComponent<MeshRenderer>().material.color = nextColor;
                    ball.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material.color = nextColor;
                    bs.MyColor = nextColor;
                    tempColor = nextColor;
                    do
                        nextColor = colors[Random.Range(0, colors.Count)];
                    while (color_Counts[nextColor] == 0);                               // ���� ���� ���� ���������� �����ϴ��� üũ
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

            ShootingLine();                                                             // ���ؼ� �׸���

            // �߻� ������
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
                    cor = StartCoroutine(TimeOut());                                    // TimeOut() �ڷ�ƾ ����
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
    /// ���� �������� �ʰ� �ָ� ���ư��� ���� �ð� �Ŀ� �����մϴ�.
    /// </summary>
    /// <returns></returns>
    IEnumerator TimeOut()
    {
        Coroutine cor2 = StartCoroutine(Reloaded());                                    // Reloaded() �ڷ�ƾ ����

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
    /// ���� �����ؾ� �ϴ� ���·� ��ȯ�մϴ�.
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
