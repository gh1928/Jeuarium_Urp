using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 공 오브젝트 스크립트가 참조할 기준 클래스입니다.
/// </summary>
public class BallBase : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            CollideWithBall(collision);
        }

        else if (collision.collider.CompareTag("Untagged") || collision.collider.CompareTag("Finish"))
        {
            CollideWithNothing(collision);
        }
    }

    public virtual void CollideWithBall(Collision collision)
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public virtual void CollideWithNothing(Collision collision)
    {
        BallShooting.life--;
        Destroy(gameObject);
    }

    /// <summary>
    /// 파티클 효과를 작동시킵니다.
    /// </summary>
    /// <param name="obj">Particle System이 부착된 게임 오브젝트의 인스턴스화</param>
    /// <param name="color">파티클 색</param>
    protected virtual void StartParticle(GameObject obj, Color color)
    {
        color.a = 1;
        ParticleSystem ps = obj.GetComponent<ParticleSystem>();
        var c = ps.main;
        var t = ps.trails;
        c.startColor = color;
        t.colorOverLifetime = color;
        ps.Play();
    }
}
