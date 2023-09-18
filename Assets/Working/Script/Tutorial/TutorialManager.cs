using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum Tutorial
{
    none,
    Scratch,
    Frottage,
    Collage,
}

public class TutorialManager : MonoBehaviour
{
    public GameObject[] TutorialUI, ScratchTutorialUI, FrottageTutorialUI, CollageTutorialUI;
    private GameObject[] CurrentTutorialUI;

    public GameObject TutorialArea, Menu, StoryCharacter;

    public Tutorial isTutorial;
    public bool isGrabTutorial;
    void Start()
    {
        int tutorialsDone = PlayerPrefs.GetInt("TutorialsDone", 0);
        /*
        if (tutorialsDone == 0)
        {
            isTutorial = Tutorial.none;
            CurrentTutorialUI = TutorialUI;
            CurrentTutorialUI[0].SetActive(true);

            Menu.SetActive(false);
            StoryCharacter.SetActive(false);
        }*/
    }


    public void SetNextTutorial(int currentTutorialNum)
    {
        switch (isTutorial)
        {
            case Tutorial.none:
                switch (currentTutorialNum)
                {
                    case 0:
                        SetNextTutorialUI(currentTutorialNum++);
                        SetNextTutorial(currentTutorialNum);
                        break;
                    case 1:
                    case 2:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3, true));
                        break;
                    case 3:
                        StartCoroutine(CheckStickTutorialCoroutine(currentTutorialNum));
                        break;
                    case 4:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3, true));
                        break;
                    case 5:
                        StartCoroutine(CheckStickTutorialCoroutine(currentTutorialNum));
                        break;
                    case 6:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3, true));
                        break;
                    case 7:
                        StartCoroutine(CheckStickTutorialCoroutine(currentTutorialNum));
                        break;
                    case 8:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3, true));
                        break;
                    case 9:
                        TutorialArea.SetActive(true);
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3, true));
                        break;
                    case 10:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3, true));
                        break;
                    case 11:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3));
                        break;
                    case 12:
                        SetNextTutorialUI(currentTutorialNum++);
                        break;
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3, true));
                        break;
                    case 17:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3));
                        break;
                    case 18:
                    case 19:
                    case 20:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3, true));
                        break;
                    case 21:
                        Menu.SetActive(true);
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3, true));
                        break;
                    case 22:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3, true));
                        break;
                    case 23:
                        TutorialArea.SetActive(false);
                        StoryCharacter.SetActive(true);
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3));
                        break;
                    default:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3, true));
                        break;
                }
                break;
            case Tutorial.Scratch:
                switch (currentTutorialNum)
                {
                    case 0:
                        SetNextTutorialUI(currentTutorialNum++);
                        SetNextTutorial(currentTutorialNum);
                        break;
                    case 1:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 4, true));
                        break;
                    case 2:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 30, true));
                        break;
                    case 3:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 30, true));
                        break;
                    case 4:
                        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 4, true));
                        break;
                }
                break;
            case Tutorial.Frottage: 
                break;
            case Tutorial.Collage: 
                break;
        }
    }

    void SetNextTutorialUI(int currentTutorialNum)
    {
        CurrentTutorialUI[currentTutorialNum++].SetActive(false);
        if (currentTutorialNum < CurrentTutorialUI.Length)
        {
            CurrentTutorialUI[currentTutorialNum].SetActive(true);
        }
        else
        {
            Debug.Log(currentTutorialNum+"튜토리얼종료");
            //마지막단계
            //튜토리얼종료
            //PlayerPrefs.SetInt("TutorialsDone", 1);
        }
    }

    public IEnumerator SetNextTutorialUICoroutine(int currentTutorialNum, float time,bool isNext=false)
    {
        yield return new WaitForSeconds(time);
        SetNextTutorialUI(currentTutorialNum++);
        if(isNext)
            SetNextTutorial(currentTutorialNum);
        else
            isGrabTutorial = true;
    }


    IEnumerator CheckStickTutorialCoroutine(int currentTutorialNum)
    {
        bool isStickMove = false;
        bool isTeleportReady = false;

        while (!isStickMove)
        {
            switch (currentTutorialNum)//move,rotate,teleport
            {
                case 3:
                    if (InputBridge.Instance.LeftThumbstickAxis != Vector2.zero)
                        isStickMove = true;
                    break;
                case 5:
                    if (InputBridge.Instance.RightThumbstickAxis != Vector2.zero)
                        isStickMove = true;
                    break;
                case 7:
                    PlayerTeleport playerTeleport = FindObjectOfType<PlayerTeleport>();

                    if (playerTeleport.enabled) //텔레포트 모드
                    {
                        CurrentTutorialUI[currentTutorialNum].GetComponentInChildren<Text>().text = "이제 스틱을 기울여서 이동해 보세요";
                        if (InputBridge.Instance.LeftThumbstickAxis != Vector2.zero)
                            isTeleportReady = true;
                        if (isTeleportReady && InputBridge.Instance.LeftThumbstickAxis == Vector2.zero)
                            isStickMove = true;
                    }
                    break;
            }

            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f);

        CurrentTutorialUI[currentTutorialNum].GetComponentInChildren<Text>().text = "잘했어요!";
        StartCoroutine(SetNextTutorialUICoroutine(currentTutorialNum, 3,true));
    }


    public GameObject Scratch, Frottage, Collage;
    public void ToolTutorial(int tool)
    {
        switch (tool)
        {
            case 0:
                isTutorial = Tutorial.Scratch;
                Scratch.SetActive(true);
                CurrentTutorialUI = ScratchTutorialUI;
                SetNextTutorial(0);
                break;
            case 1:
                break;
            case 2:
                break;

        }
    }


}
