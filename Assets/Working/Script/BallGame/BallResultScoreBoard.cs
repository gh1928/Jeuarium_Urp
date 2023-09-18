using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class BallResultScoreBoard : MonoBehaviour
{
    public static BallResultScoreBoard instance;

    [Tooltip("���â���� ����/���� ���θ� ���� ������ UI")]
    public GameObject scoreboard_IntroUI;

    [Tooltip("���â���� ����� ǥ���� �ؽ�Ʈ ������Ʈ")]
    public TextMeshProUGUI scoreboard_TitleUI;

    [Tooltip("���â���� ������ ǥ���� �ؽ�Ʈ ������Ʈ")]
    public TextMeshProUGUI scoreboard_scoreUI;

    [Tooltip("���â���� ����ð��� ǥ���� �ؽ�Ʈ ������Ʈ")]
    public TextMeshProUGUI scoreboard_spendTimeUI;

    [Tooltip("���â���� ������ ���� Ƚ���� ǥ���� �ؽ�Ʈ ������Ʈ")]
    public TextMeshProUGUI scoreboard_throwCountUI;

    [Tooltip("���â���� ������ ���� Ƚ���� ǥ���� �ؽ�Ʈ ������Ʈ")]
    public TextMeshProUGUI scoreboard_HighScore;

    [Tooltip("���â���� ������ ���� Ƚ���� ǥ���� �ؽ�Ʈ ������Ʈ")]
    public TextMeshProUGUI scoreboard_History1;

    [Tooltip("���â���� ������ ���� Ƚ���� ǥ���� �ؽ�Ʈ ������Ʈ")]
    public TextMeshProUGUI scoreboard_History2;

    [Tooltip("���â���� ������ ���� Ƚ���� ǥ���� �ؽ�Ʈ ������Ʈ")]
    public TextMeshProUGUI scoreboard_History3;

    [Tooltip("���â���� ������ ���� Ƚ���� ǥ���� �ؽ�Ʈ ������Ʈ")]
    public TextMeshProUGUI scoreboard_History4;

    [Tooltip("���â���� �������� �Ѿ�� ��ư ������Ʈ")]
    public Button scoreboard_NextUI;

    [Tooltip("���â���� �ش� ���� �ٽ� �÷����ϴ� ��ư ������Ʈ")]
    public Button scoreboard_AgainUI;

    [Tooltip("�󸶸�ŭ�� ������ ���� ���� ���������� ����ϴ����� ���մϴ�.")]
    public int score_Limit = 0;

    [Tooltip("���� ������ ������ PlayerPrefs Ű �̸��� �����մϴ�.")]
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
                builder.Append("��");
            }
            else
            {
                int min = (spendTime / 60);
                builder.Append(min.ToString());
                builder.Append("�� ");
                builder.Append((spendTime - (min*60)).ToString());
                builder.Append("��");
            }
            scoreboard_spendTimeUI.text = builder.ToString();
            builder.Clear();
        }

        if (BallShooting.score < score_Limit || isFail)
        {
            scoreboard_TitleUI.text = "����!";
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
