using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class BallResultScoreBoard : MonoBehaviour
{
    public static BallResultScoreBoard instance;

    [Tooltip("결과창에서 성공/실패 여부를 먼저 보여줄 UI")]
    public GameObject scoreboard_IntroUI;

    [Tooltip("결과창에서 결과를 표시할 텍스트 컴포넌트")]
    public TextMeshProUGUI scoreboard_TitleUI;

    [Tooltip("결과창에서 점수를 표시할 텍스트 컴포넌트")]
    public TextMeshProUGUI scoreboard_scoreUI;

    [Tooltip("결과창에서 경과시간을 표시할 텍스트 컴포넌트")]
    public TextMeshProUGUI scoreboard_spendTimeUI;

    [Tooltip("결과창에서 버블을 던진 횟수를 표시할 텍스트 컴포넌트")]
    public TextMeshProUGUI scoreboard_throwCountUI;

    [Tooltip("결과창에서 버블을 던진 횟수를 표시할 텍스트 컴포넌트")]
    public TextMeshProUGUI scoreboard_HighScore;

    [Tooltip("결과창에서 버블을 던진 횟수를 표시할 텍스트 컴포넌트")]
    public TextMeshProUGUI scoreboard_History1;

    [Tooltip("결과창에서 버블을 던진 횟수를 표시할 텍스트 컴포넌트")]
    public TextMeshProUGUI scoreboard_History2;

    [Tooltip("결과창에서 버블을 던진 횟수를 표시할 텍스트 컴포넌트")]
    public TextMeshProUGUI scoreboard_History3;

    [Tooltip("결과창에서 버블을 던진 횟수를 표시할 텍스트 컴포넌트")]
    public TextMeshProUGUI scoreboard_History4;

    [Tooltip("결과창에서 다음으로 넘어가는 버튼 컴포넌트")]
    public Button scoreboard_NextUI;

    [Tooltip("결과창에서 해당 씬을 다시 플레이하는 버튼 컴포넌트")]
    public Button scoreboard_AgainUI;

    [Tooltip("얼마만큼의 점수를 내야 다음 스테이지로 통과하는지를 정합니다.")]
    public int score_Limit = 0;

    [Tooltip("현재 점수를 저장할 PlayerPrefs 키 이름을 지정합니다.")]
    public string score_Prefs;

    [HideInInspector]
    public bool isFail = false;

    void Start()
    {
        instance = this;        
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void ActiveBoard()
    {
        Transform player = Camera.main.transform;
        Vector3 pos = player.position + (Vector3.Cross(player.right, Vector3.up)).normalized * 1f;
        transform.position = pos;
        transform.localRotation = Quaternion.LookRotation((pos- player.position).normalized, Vector3.up);
        if (int.TryParse( BallShooting.spendTime.ToString("F0"), out int spendTime)){

            StringBuilder builder = new StringBuilder();
            if (spendTime < 60)
            {
                builder.Append(spendTime.ToString());
                builder.Append("초");
            }
            else
            {
                int min = (spendTime / 60);
                builder.Append(min.ToString());
                builder.Append("분 ");
                builder.Append((spendTime - (min*60)).ToString());
                builder.Append("초");
            }
            scoreboard_spendTimeUI.text = builder.ToString();
            builder.Clear();
        }

        if (BallShooting.score < score_Limit || isFail)
        {
            scoreboard_TitleUI.text = "실패!";
            scoreboard_AgainUI.gameObject.SetActive(true);
            scoreboard_IntroUI.transform.GetChild(0).gameObject.SetActive(false);
            scoreboard_IntroUI.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            scoreboard_NextUI.gameObject.SetActive(true);
        }

        if (BallShooting.score > PlayerPrefs.GetInt(score_Prefs))
        {
            PlayerPrefs.SetInt(score_Prefs, (int)BallShooting.score);
            PlayerPrefs.Save();
        }

        scoreboard_HighScore.text = PlayerPrefs.GetInt(score_Prefs).ToString();

        for (int i = 4; i > 1; i--)
        {
            PlayerPrefs.SetInt(score_Prefs + "_" + i.ToString(), PlayerPrefs.GetInt(score_Prefs + "_" + (i - 1).ToString()));
        }
        PlayerPrefs.SetInt(score_Prefs + "_1", (int)BallShooting.score);

        scoreboard_History1.text = PlayerPrefs.GetInt(score_Prefs + "_1").ToString();
        scoreboard_History2.text = PlayerPrefs.GetInt(score_Prefs + "_2").ToString();
        scoreboard_History3.text = PlayerPrefs.GetInt(score_Prefs + "_3").ToString();
        scoreboard_History4.text = PlayerPrefs.GetInt(score_Prefs + "_4").ToString();

        transform.GetChild(0).GetComponent<Animator>().SetTrigger("isPlay");
    }
}
