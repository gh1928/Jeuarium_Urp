using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallSpawn : MonoBehaviour
{
    public enum StageType       // 스테이지 공 배열 방식
    {
        Cube,
        Pyramid,
        Sphere
    };

    [Tooltip("BallShooting 컴포넌트를 지정합니다.")]
    public BallShooting ballShooting;

    [Tooltip("스테이지에 배치할 공의 프리팹을 지정합니다.")]
    public GameObject prefab_Ball;

    [Tooltip("스테이지의 공 배열 방식을 정합니다.")]
    public StageType stageType = StageType.Cube;

    [Tooltip("스테이지의 공이 가질 색깔을 정합니다.")]
    public List<Color> colors;

    [Tooltip("스테이지 생성 시 처음부터 체인을 이루지 않게 만들지의 여부를 정합니다. 색 종류가 일정 개수 이상일 때에만 사용하십시오.")]
    public bool isNotchain = false;

    Vector3 border;
    Transform ball;

    List<Transform> checkedBalls = new List<Transform>();

    void Awake()
    {
        BallShooting.ballSpawnQueue.Enqueue(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            GameOver();
        }
    }

    private void FixedUpdate()
    {
        if (BallShooting.life == 0)
        {
            GameOver();
        }
    }

    public void Initialize()
    {
        int[] tempArray = new int[colors.Count];
        tempArray.Initialize();

        border = GetComponent<BoxCollider>().bounds.size;
        float r = prefab_Ball.transform.lossyScale.x;
        switch (stageType)
        {
            case StageType.Cube:
                MakeCube(border, r, false);
                break;
            case StageType.Pyramid:
                MakePyramid(border, r, false);
                break;
            case StageType.Sphere:
                MakeSphere(border, r, false);
                break;
        }
    }

    /// <summary>
    /// 공을 큐브 모양으로 배열합니다.
    /// </summary>
    /// <param name="size">스테이지 크기</param>
    /// <param name="radius">공의 지름</param>
    private void MakeCube(Vector3 size, float radius, bool isGizmos)
    {
        for (int i = 0; i < Mathf.Floor(size.y / radius); i++)
        {
            for (int j = 0; j < Mathf.Floor(size.x / radius); j++)
            {
                for (int k = 0; k < Mathf.Floor(size.z / radius); k++)
                {
                    Vector3 pos = new Vector3((radius / 2 + j * radius) - size.x / 2, (radius / 2 + i * radius) - size.y / 2, (radius / 2 + k * radius) - size.z / 2);
                    if (isGizmos)
                        Gizmos.DrawMesh(prefab_Ball.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh, transform.position + pos, Quaternion.identity, 0.05f * Vector3.one);
                    else
                        SpawnBall(pos);
                }
            }
        }
    }

    /// <summary>
    /// 공을 피라미드 모양으로 배열합니다.
    /// </summary>
    /// <param name="size">스테이지 크기</param>
    /// <param name="radius">공의 지름</param>
    private void MakePyramid(Vector3 size, float radius, bool isGizmos)
    {
        for (int i = 0; i < Mathf.Floor(size.y / radius); i++)
        {
            for (int j = 0; j < Mathf.Floor(size.x / radius) - i; j++)
            {
                for (int k = 0; k < Mathf.Floor(size.z / radius) - i; k++)
                {
                    Vector3 pos = new Vector3((radius / 2 + j * radius) - size.x / 2 + i * radius / 2, (radius / 2 + i * radius) - size.y / 2, (radius / 2 + k * radius) - size.z / 2 + i * radius / 2);
                    if (isGizmos)
                        Gizmos.DrawMesh(prefab_Ball.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh, transform.position + pos, Quaternion.identity, 0.05f * Vector3.one);
                    else
                        SpawnBall(pos);
                }
            }
        }
    }

    /// <summary>
    /// 공을 구 모양으로 배열합니다.
    /// </summary>
    /// <param name="size">스테이지 크기</param>
    /// <param name="radius">공의 지름</param>
    private void MakeSphere(Vector3 size, float radius, bool isGizmos)
    {
        for (int i = 0; i < Mathf.Floor(size.y / radius); i++)
        {
            for (int j = 0; j < Mathf.Floor(size.x / radius); j++)
            {
                for (int k = 0; k < Mathf.Floor(size.z / radius); k++)
                {
                    Vector3 pos = new Vector3((radius / 2 + j * radius) - size.x / 2, (radius / 2 + i * radius) - size.y / 2, (radius / 2 + k * radius) - size.z / 2);
                    if (Vector3.Distance(transform.position + pos, transform.position) <= size.x / 2)
                    {
                        if (isGizmos)
                            Gizmos.DrawMesh(prefab_Ball.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh, transform.position + pos, Quaternion.identity, 0.05f * Vector3.one);
                        else
                            SpawnBall(pos);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 지정된 위치에 공을 생성합니다.
    /// </summary>
    /// <param name="pos">공 생성 위치</param>
    private void SpawnBall(Vector3 pos)
    {
        ball = Instantiate(prefab_Ball, transform.position + pos, Quaternion.identity, transform).transform.GetChild(0);
        BallScore bs = ball.parent.GetComponent<BallScore>();
        bs.collidedThresholdCount = ballShooting.chain;
        Color temp = colors[UnityEngine.Random.Range(0, colors.Count)];

        if (isNotchain)
        {
            checkedBalls.Clear();
            while (CheckColors(ballShooting.chain - 1, ball.parent, temp) == 0)
            {
                checkedBalls.Clear();
                temp = colors[UnityEngine.Random.Range(0, colors.Count)];
            }
        }

        AddColorBall(temp);
        ball.GetComponent<MeshRenderer>().material.color = temp;
        ball.GetChild(1).GetComponent<MeshRenderer>().material.color = temp;
        bs.MyColor = temp;
    }

    public void AddColorBall(Color ballColor)
    {
        if (BallShooting.color_Counts.ContainsKey(ballColor))
        {
            BallShooting.color_Counts[ballColor]++;
        }
        else
        {
            BallShooting.color_Counts.Add(ballColor, 1);
        }
    }

    public void RemoveColorBall(Color ballColor)
    {
        if (BallShooting.color_Counts.ContainsKey(ballColor))
        {
            BallShooting.color_Counts[ballColor]--;
        }
        else
        {
            Debug.Log("This color is out of dictionary");
        }
    }

    /// <summary>
    /// 처음부터 일정 개수의 공이 맞닿지 않도록 검사합니다.
    /// </summary>
    /// <param name="chain_Num">남은 체인의 수</param>
    /// <param name="pivot">검사 기준 위치</param>
    /// <param name="c">검사 기준 색</param>
    /// <returns></returns>
    private int CheckColors(int chain_Num, Transform pivot, Color c)
    {
        checkedBalls.Add(pivot);
        if (chain_Num == 0) return 0;

        Collider[] colList = Physics.OverlapSphere(pivot.position, prefab_Ball.transform.lossyScale.x, 1 << LayerMask.NameToLayer("Ball"));

        foreach (Collider col in colList)
        {
            if (checkedBalls.Contains(col.transform)) continue;

            else if (col.transform.GetChild(0).GetComponent<MeshRenderer>().material.color == c)
            {
                chain_Num = CheckColors(chain_Num - 1, col.transform, c);

                if (chain_Num == 0)
                {
                    return 0;
                }
            }
        }

        return chain_Num;
    }

    private void GameOver()
    {
        BallResultScoreBoard.instance.isFail = true;
        BallResultScoreBoard.instance.ActiveBoard();
        Destroy(ballShooting.gameObject);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        border = GetComponent<BoxCollider>().bounds.size;
        float r = prefab_Ball.transform.lossyScale.x;
        switch (stageType)
        {
            case StageType.Cube:
                MakeCube(border, r, true);
                break;
            case StageType.Pyramid:
                MakePyramid(border, r, true);
                break;
            case StageType.Sphere:
                MakeSphere(border, r, true);
                break;
        }
    }
}
