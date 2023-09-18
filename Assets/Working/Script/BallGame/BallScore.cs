using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallScore : BallBase
{
    [Tooltip("���� ���� �� ����� ȿ���� �����մϴ�.")]
    public GameObject prefab_Fx;

    [Tooltip("������ ���� üũ�� ��, üũ������ �����ݰ��� �����մϴ�.")]
    public float collideRadiusOffset = 0.0f;

    [HideInInspector]
    public Vector3 pre_Velocity = Vector3.zero;
    [HideInInspector]
    public int collidedThresholdCount = 3;

    BallSpawn ballSpawn;
    AudioSource audioSource;

    Vector3 velocity = Vector3.zero;
    bool isCrush = false;

    Color myColor;
    public Color MyColor { get { return myColor; } set { myColor = value; } }

    List<Transform> checkedBalls = new List<Transform>();

    private void FixedUpdate()
    {
        pre_Velocity = velocity;                            // BallReflect���� ������ "OnCollisionEnter �浹 ���� �ӵ�"�� ����
        velocity = GetComponent<Rigidbody>().velocity;

        if (isCrush)                                        // ���� ���������� �浹 ��, ���� ���� ���� �ε��� �ߺ� ����Ǵ� ��� ������
        {
            GetComponent<SphereCollider>().isTrigger = true;
            int chain_Score = BallCheck(1, transform);
            if (chain_Score >= collidedThresholdCount)      // ���� ü�� �� ��ŭ ���� ���� ���� �´�� ���� ���...
            {
                BallShooting.AddScore(Mathf.Min(Mathf.Pow(2, chain_Score - 3), 8) * 100);
                foreach (Transform ball in checkedBalls)
                {
                    if (ball == transform) continue;

                    ballSpawn.RemoveColorBall(myColor);
                    StartParticle(Instantiate(prefab_Fx, ball.transform.position, Quaternion.identity), myColor);
                    Destroy(ball.gameObject);
                }
                audioSource.Play();
                StartParticle(Instantiate(prefab_Fx, transform.position, Quaternion.identity), myColor);
                Destroy(gameObject);
            }
            else
            {
                checkedBalls.Clear();
                transform.tag = "Ball";
                GetComponent<SphereCollider>().isTrigger = false;
                gameObject.layer = LayerMask.NameToLayer("Ball");
            }
            isCrush = false;
        }
    }

    public override void CollideWithBall(Collision collision)
    {
        ballSpawn = collision.transform.parent.GetComponent<BallSpawn>();
        audioSource = ballSpawn.GetComponent<AudioSource>();
        transform.parent = collision.transform.parent;
        isCrush = true;

        base.CollideWithBall(collision);
    }

    public override void CollideWithNothing(Collision collision)
    {
        StartParticle(Instantiate(prefab_Fx, transform.position, Quaternion.identity), transform.GetChild(0).GetComponent<MeshRenderer>().material.color);

        base.CollideWithNothing(collision);
    }

    /// <summary>
    /// ��ó�� ���� ������ ���� �´�� �ִ��� üũ�մϴ�.
    /// </summary>
    public int BallCheck(int chain_Num, Transform collider)
    {
        checkedBalls.Add(collider);

        Collider[] colList = Physics.OverlapSphere(collider.position, collider.lossyScale.x + collideRadiusOffset, 1 << LayerMask.NameToLayer("Ball"));

        foreach (Collider col in colList)
        {
            if (checkedBalls.Contains(col.transform)) continue;
            
            else if (col.transform.GetChild(0).GetComponent<MeshRenderer>().material.color == myColor)
            {
                chain_Num = BallCheck(chain_Num + 1, col.transform);
                
                if (chain_Num == 0)
                {
                    return 0;
                }
            }
        }

        return chain_Num;
    }
}
