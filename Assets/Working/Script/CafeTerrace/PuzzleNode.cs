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

    private PuzzleDir currDir = PuzzleDir.None;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    public void SetPlayerColor(Color color) => playerColor = color;

    public void SetColor()
    {
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
        nodeImage.color = Color.white;
    }

    public bool IsVisited() => visited;

    public void UpdateLine(Vector2 inputMoveDelta)
    {
        if (currDir == PuzzleDir.None)
            return;

        float lineSizeChanger = 0;
        switch (currDir)
        {
            case PuzzleDir.Top:
                lineSizeChanger = inputMoveDelta.y;
                break;
            case PuzzleDir.Right:
                lineSizeChanger = inputMoveDelta.x;
                break;
            case PuzzleDir.Bottom:
                lineSizeChanger = -inputMoveDelta.y;
                break;
            case PuzzleDir.Left:
                lineSizeChanger = -inputMoveDelta.x;
                break;
        }

        currLine.fillAmount += lineSizeChanger;
    }

    public void SetPathable(PuzzleDir dir, bool pathable) => pathUseable[(int)dir] = pathable;
    public bool GetPathable(PuzzleDir dir) => pathUseable[(int)dir];
    public void DisableTopPath() => topPath.gameObject.SetActive(false);
    public void DisableRightPath() => rightPath.gameObject.SetActive(false);
}
