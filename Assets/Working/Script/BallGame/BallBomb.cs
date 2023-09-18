using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallBomb : BallBase
{
    [Tooltip("폭탄을 제외한 공이 터질 때 재생될 효과를 지정합니다.")]
    public GameObject prefab_Fx;

    [Tooltip("폭탄이 터지는 반경의 배수를 정합니다.")]
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
        // scale에 해당하는 반경 안의 공을 모두 터뜨리고 파티클을 재생합니다.
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
