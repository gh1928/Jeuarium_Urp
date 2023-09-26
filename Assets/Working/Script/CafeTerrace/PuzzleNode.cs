using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PuzzleDir
{
    None = -1, Top = 0, Right = 1, Bottom = 2, Left = 3
}

public class PuzzleNode : MonoBehaviour
{
    public int NodeNumber;

    public (int posY, int posX) Pos;

    private RectTransform rect;

    public RectTransform topPath;
    public RectTransform rightPath;

    public Image nodeImage;
    public Image[] lines = new Image[4];
    private Image currLine;

    private bool[] pathUseable = { true, true, true, true };

    private bool visited = false;

    private Color playerColor;
    private Color baseColor;

    private PuzzleDir currDir = PuzzleDir.None;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    public void SetPlayerColor(Color color) => playerColor = color;
    public void SetBaseColor(Color color) => baseColor = color;

    public void SetColor()
    {
        nodeImage.color = baseColor;
        topPath.GetComponent<RawImage>().color = baseColor;
        rightPath.GetComponent<RawImage>().color = baseColor;

        for (int i = 0; i < 4; i++)
            lines[i].color = playerColor;
    }
    public void SetDestDir(PuzzleDir dir)
    {
        currDir = dir;
        currLine = lines[(int)currDir];
    }
    public PuzzleDir GetOppositeDirection() => currDir switch
    {
        PuzzleDir.None => PuzzleDir.None,
        PuzzleDir.Top => PuzzleDir.Bottom,
        PuzzleDir.Right => PuzzleDir.Left,
        PuzzleDir.Bottom => PuzzleDir.Top,
        PuzzleDir.Left => PuzzleDir.Right,
        _ => PuzzleDir.None,
    };

    public float GetProgress() => currLine == null ? 0f : currLine.fillAmount;    
    public void SetProgress(float value) => currLine.fillAmount = value;
    public void ClampProgress(float maxValue)
    {
        if(currLine.fillAmount > maxValue)
            currLine.fillAmount = maxValue;
    }

    public void SetPathAndLine(float interval)
    {
        float currSize = rect.sizeDelta.y;

        topPath.transform.position = transform.position + 0.5f * interval * Vector3.up;
        topPath.sizeDelta = new Vector2(currSize, interval);

        rightPath.transform.position = transform.position + 0.5f * interval * Vector3.right;
        rightPath.sizeDelta = new Vector2(interval, currSize);

        Vector3[] dirs = new Vector3[4];
        dirs[0] = Vector3.up;
        dirs[1] = Vector3.right;
        dirs[2] = Vector3.down;
        dirs[3] = Vector3.left;

        for (int i = 0; i < 4; i++)
        {
            bool isVertical = i % 2 == 0;

            lines[i].transform.position = transform.position + 0.5f * interval * dirs[i];
            lines[i].rectTransform.sizeDelta = new Vector2(isVertical ? currSize : interval, isVertical ? interval : currSize);
        }
    }
    public void OnVisitAction()
    {
        visited = true;
        nodeImage.color = playerColor;
    }
    public void CancleVisit()
    {
        visited = false;
        nodeImage.color = baseColor;
    }

    public bool IsVisited() => visited;

    //고정 속도
    public void UpdateLine(Vector2 pointerDir, float value)
    {
        if (currDir == PuzzleDir.None)
            return;

        if(pointerDir == Vector2.zero) 
            return;

        bool valuePositive = true;

        switch (currDir)
        {
            case PuzzleDir.Top:
                valuePositive = pointerDir.y > 0;
                break;
            case PuzzleDir.Bottom:
                valuePositive = pointerDir.y < 0;
                break;
            case PuzzleDir.Right:
                valuePositive = pointerDir.x < 0;
                break;
            case PuzzleDir.Left:
                valuePositive = pointerDir.x > 0;
                break;
        }

        currLine.fillAmount += valuePositive ? value : - value;
    }

    //포인터 거리 비례 속도 증가
    public void UpdateLine(Vector2 moveValue)
    {
        if (currDir == PuzzleDir.None)
            return;

        if (moveValue == Vector2.zero)
            return;

        float lineSizeChanger = 0f;

        switch (currDir)
        {
            case PuzzleDir.Top:
                lineSizeChanger = moveValue.y;
                break;
            case PuzzleDir.Bottom:
                lineSizeChanger = - moveValue.y;                
                break;
            case PuzzleDir.Right:
                lineSizeChanger = -moveValue.x;
                break;
            case PuzzleDir.Left:
                lineSizeChanger =  moveValue.x;
                break;
        }

        currLine.fillAmount += lineSizeChanger;
    }

    public void SetPathable(PuzzleDir dir, bool pathable) => pathUseable[(int)dir] = pathable;
    public bool GetPathable(PuzzleDir dir) => pathUseable[(int)dir];
    public void DisableTopPath() => topPath.gameObject.SetActive(false);
    public void DisableRightPath() => rightPath.gameObject.SetActive(false);
}
