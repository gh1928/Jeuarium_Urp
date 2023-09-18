using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallBomb : BallBase
{
    [Tooltip("��ź�� ������ ���� ���� �� ����� ȿ���� �����մϴ�.")]
    public GameObject prefab_Fx;

    [Tooltip("��ź�� ������ �ݰ��� ����� ���մϴ�.")]
    public float scale = 1.0f;

    BallSpawn ballSpawn;
    AudioSource audioSource;

    bool isCrush = false;

    public override void CollideWithBall(Collision collision)
    {
        ballSpawn = collision.transform.parent.GetComponent<BallSpawn>();
        audioSource = ballSpawn.GetComponent<AudioSource>();
        isCrush = true;

        base.CollideWithBall(collision);
    }

    public override void CollideWithNothing(Collision collision)
    {
        base.CollideWithNothing(collision);
    }

    private void FixedUpdate()
    {
        // scale�� �ش��ϴ� �ݰ� ���� ���� ��� �Ͷ߸��� ��ƼŬ�� ����մϴ�.
        if (isCrush)
        {
            Collider[] colList = Physics.OverlapSphere(transform.position, transform.lossyScale.x * scale, 1 << LayerMask.NameToLayer("Ball"));
            BallShooting.AddScore(colList.Length * 50);
            foreach (Collider col in colList)
            {
                Color c = col.GetComponent<BallScore>().MyColor;
                ballSpawn.RemoveColorBall(c);
                StartParticle(Instantiate(prefab_Fx, col.transform.position, Quaternion.identity), c);
                Destroy(col.gameObject);
            }
            audioSource.Play();
            Destroy(gameObject);
        }
    }
}
