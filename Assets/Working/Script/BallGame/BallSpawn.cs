using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallSpawn : MonoBehaviour
{
    public enum StageType       // �������� �� �迭 ���
    {
        Cube,
        Pyramid,
        Sphere
    };

    [Tooltip("BallShooting ������Ʈ�� �����մϴ�.")]
    public BallShooting ballShooting;

    [Tooltip("���������� ��ġ�� ���� �������� �����մϴ�.")]
    public GameObject prefab_Ball;

    [Tooltip("���������� �� �迭 ����� ���մϴ�.")]
    public StageType stageType = StageType.Cube;

    [Tooltip("���������� ���� ���� ������ ���մϴ�.")]
    public List<Color> colors;

    [Tooltip("�������� ���� �� ó������ ü���� �̷��� �ʰ� �������� ���θ� ���մϴ�. �� ������ ���� ���� �̻��� ������ ����Ͻʽÿ�.")]
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
    /// ���� ť�� ������� �迭�մϴ�.
    /// </summary>
    /// <param name="size">�������� ũ��</param>
    /// <param name="radius">���� ����</param>
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
    /// ���� �Ƕ�̵� ������� �迭�մϴ�.
    /// </summary>
    /// <param name="size">�������� ũ��</param>
    /// <param name="radius">���� ����</param>
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
    /// ���� �� ������� �迭�մϴ�.
    /// </summary>
    /// <param name="size">�������� ũ��</param>
    /// <param name="radius">���� ����</param>
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
    /// ������ ��ġ�� ���� �����մϴ�.
    /// </summary>
    /// <param name="pos">�� ���� ��ġ</param>
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
    /// ó������ ���� ������ ���� �´��� �ʵ��� �˻��մϴ�.
    /// </summary>
    /// <param name="chain_Num">���� ü���� ��</param>
    /// <param name="pivot">�˻� ���� ��ġ</param>
    /// <param name="c">�˻� ���� ��</param>
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
