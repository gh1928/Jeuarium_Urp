using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� �� ������Ʈ ��ũ��Ʈ�� ������ ���� Ŭ�����Դϴ�.
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
    /// ��ƼŬ ȿ���� �۵���ŵ�ϴ�.
    /// </summary>
    /// <param name="obj">Particle System�� ������ ���� ������Ʈ�� �ν��Ͻ�ȭ</param>
    /// <param name="color">��ƼŬ ��</param>
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
