using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BallRealtimeScore : MonoBehaviour
{
    public TextMeshProUGUI score_UI;
    public TextMeshProUGUI score_LimitUI;
    public Transform lifes;
    int life_now = 3;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => { return BallResultScoreBoard.instance != null; });
        score_LimitUI.text = BallResultScoreBoard.instance.score_Limit.ToString();
        life_now = BallShooting.life;
    }

    void FixedUpdate()
    {
        if (BallShooting.instance == null)
        {
            Destroy(gameObject);
        }

        score_UI.text = ((int)BallShooting.score).ToString();

        if (BallShooting.life != life_now && BallShooting.life != 0)
        {
            Destroy(lifes.GetChild(0).gameObject);
            life_now = BallShooting.life;
        }
    }
}
