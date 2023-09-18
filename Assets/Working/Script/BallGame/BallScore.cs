using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallScore : BallBase
{
    [Tooltip("공이 터질 때 재생될 효과를 지정합니다.")]
    public GameObject prefab_Fx;

    [Tooltip("겹쳐진 공을 체크할 때, 체크범위의 여유반경을 지정합니다.")]
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
        pre_Velocity = velocity;                            // BallReflect에서 참조할 "OnCollisionEnter 충돌 직전 속도"를 저장
        velocity = GetComponent<Rigidbody>().velocity;

        if (isCrush)                                        // 공이 스테이지에 충돌 시, 여러 개의 공과 부딪혀 중복 실행되는 경우 방지함
        {
            GetComponent<SphereCollider>().isTrigger = true;
            int chain_Score = BallCheck(1, transform);
            if (chain_Score >= collidedThresholdCount)      // 일정 체인 수 만큼 같은 색의 공이 맞닿아 있을 경우...
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
    /// 근처에 일정 개수의 공이 맞닿아 있는지 체크합니다.
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
