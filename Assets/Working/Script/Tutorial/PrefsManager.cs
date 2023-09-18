using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefsManager : MonoBehaviour
{
    public int limit1 = 0;
    public int limit2 = 0;
    public int limit3 = 0;

    public List<GameObject> monet1;
    public List<GameObject> monet2;
    public List<GameObject> monet3;

    void Start()
    {
        foreach (GameObject g in monet1)
        {
            g.SetActive(false);
        }

        foreach (GameObject g in monet2)
        {
            g.SetActive(false);
        }

        foreach (GameObject g in monet3)
        {
            g.SetActive(false);
        }

        if (!PlayerPrefs.HasKey("score1"))
        {
            PlayerPrefs.SetInt("score1", 0);
            PlayerPrefs.SetInt("score1_1", 0);
            PlayerPrefs.SetInt("score1_2", 0);
            PlayerPrefs.SetInt("score1_3", 0);
            PlayerPrefs.SetInt("score1_4", 0);
        }
        if (!PlayerPrefs.HasKey("score2"))
        {
            PlayerPrefs.SetInt("score2", 0);
            PlayerPrefs.SetInt("score2_1", 0);
            PlayerPrefs.SetInt("score2_2", 0);
            PlayerPrefs.SetInt("score2_3", 0);
            PlayerPrefs.SetInt("score2_4", 0);
        }
        if (!PlayerPrefs.HasKey("score3"))
        {
            PlayerPrefs.SetInt("score3", 0);
            PlayerPrefs.SetInt("score3_1", 0);
            PlayerPrefs.SetInt("score3_2", 0);
            PlayerPrefs.SetInt("score3_3", 0);
            PlayerPrefs.SetInt("score3_4", 0);
        }

        if (PlayerPrefs.GetInt("score1") >= limit1)
        {
            foreach (GameObject g in monet1)
            {
                g.SetActive(true);
            }
        }

        if (PlayerPrefs.GetInt("score2") >= limit2)
        {
            foreach (GameObject g in monet2)
            {
                g.SetActive(true);
            }
        }

        if (PlayerPrefs.GetInt("score3") >= limit3)
        {
            foreach (GameObject g in monet3)
            {
                g.SetActive(true);
            }
        }
    }
}
