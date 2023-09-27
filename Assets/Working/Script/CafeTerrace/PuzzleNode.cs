using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public enum PuzzleDir
{
    None = -1, Up = 0, Right = 1, Down = 2, Left = 3
}

public class PuzzleNode : MonoBehaviour
{
    public int NodeNumber;
    public float minProgressSpeed = 0.1f;    

    public (int posY, int posX) Pos;

    private RectTransform rect;
    private float lineWidth;
    private float lineLength;

    public RectTransform topPath;
    public RectTransform rightPath;

    public Image nodeImage;
    public Image[] lines = new Image[4];
    private BoxCollider[] lineColliders = new BoxCollider[4];
    private Vector3 colliderBaseSize;

    private Image currLine;

    private bool[] pathUseable = { true, true, true, true };

    private bool visited = false;

    private Color playerColor;
    private Color baseColor;

    private PuzzleDir currDir = PuzzleDir.None;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        for(int i = 0; i < 4; i++)
            lineColliders[i] = lines[i].GetComponent<BoxCollider>();
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

    public PuzzleDir GetDir() => currDir;

    public PuzzleDir GetOppositeDirection() => currDir switch
    {
        PuzzleDir.None => PuzzleDir.None,
        PuzzleDir.Up => PuzzleDir.Down,
        PuzzleDir.Right => PuzzleDir.Left,
        PuzzleDir.Down => PuzzleDir.Up,
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

    public void SetPathAndProgressLine(float interval)
    {
        lineWidth = rect.sizeDelta.y;
        lineLength = interval;

        topPath.transform.position = transform.position + 0.5f * lineLength * Vector3.up;
        topPath.sizeDelta = new Vector2(lineWidth, lineLength);

        rightPath.transform.position = transform.position + 0.5f * lineLength * Vector3.right;
        rightPath.sizeDelta = new Vector2(lineLength, lineWidth);

        Vector3[] dirs = new Vector3[4];
        dirs[0] = Vector3.up;
        dirs[1] = Vector3.right;
        dirs[2] = Vector3.down;
        dirs[3] = Vector3.left;

        for (int i = 0; i < 4; i++)
        {
            bool isVertical = i % 2 == 0;

            lines[i].transform.position = transform.position + 0.5f * interval * dirs[i];
            lines[i].rectTransform.sizeDelta = new Vector2(isVertical ? lineWidth : interval, isVertical ? interval : lineWidth);
        }

        colliderBaseSize = Vector3.one * lineWidth;
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
    public void UpdateProgress(Vector2 pointerDiff, float value)
    {
        if (currDir == PuzzleDir.None)
            return;

        if(pointerDiff == Vector2.zero) 
            return;

        bool valuePositive = true;

        switch (currDir)
        {
            case PuzzleDir.Up:
                valuePositive = pointerDiff.y > 0;
                break;
            case PuzzleDir.Down:
                valuePositive = pointerDiff.y < 0;
                break;
            case PuzzleDir.Right:
                valuePositive = pointerDiff.x < 0;
                break;
            case PuzzleDir.Left:
                valuePositive = pointerDiff.x > 0;
                break;
        }

        float minSpeed = minProgressSpeed * Time.deltaTime;

        if(value < minSpeed)
            value = minSpeed;        

        currLine.fillAmount += valuePositive ? value : - value;

        UpdateCollider();
    }

    //포인터 거리 비례 속도 증가
    public void UpdateProgress(Vector2 moveValue)
    {
        if (currDir == PuzzleDir.None)
            return;

        if (moveValue == Vector2.zero)
            return;

        float lineSizeChanger = 0f;

        switch (currDir)
        {
            case PuzzleDir.Up:                
            case PuzzleDir.Down:
                lineSizeChanger = moveValue.y;
                break;
            case PuzzleDir.Right:                
            case PuzzleDir.Left:
                lineSizeChanger = moveValue.x;
                break;
        }

        //float minSpeed = minProgressSpeed * Time.deltaTime;
        //lineSizeChanger = Mathf.Clamp(lineSizeChanger, -minSpeed, minSpeed);

        bool positiveAxis = (currDir == PuzzleDir.Up || currDir == PuzzleDir.Left);

        currLine.fillAmount += positiveAxis ? lineSizeChanger : -lineSizeChanger;

        UpdateCollider();
    }
    public void UpdateCollider()
    {
        Vector3 newSize = colliderBaseSize;
        float progress = currLine.fillAmount;

        Vector3 newCenter = Vector3.zero;        

        switch (currDir)
        {
            case PuzzleDir.Up:
                newSize.y = lineLength * progress;
                newCenter.y = Mathf.Lerp(-lineLength * 0.5f, 0f, progress);
                break;
            case PuzzleDir.Down:
                newSize.y = lineLength * progress; 
                newCenter.y = Mathf.Lerp(lineLength * 0.5f, 0f, progress);
                break;
            case PuzzleDir.Right:
                newSize.x = lineLength * progress;
                newCenter.x = Mathf.Lerp(- lineLength * 0.5f, 0f, progress);                
                break;
            case PuzzleDir.Left:
                newSize.x = lineLength * progress;
                newCenter.x = Mathf.Lerp(lineLength * 0.5f, 0f, progress);
                break;
        }

        BoxCollider targetCollider = lineColliders[(int)currDir];
        targetCollider.size = newSize;
        targetCollider.center = newCenter;
    }

    public void SetPathable(PuzzleDir dir, bool pathable) => pathUseable[(int)dir] = pathable;
    public bool GetPathable(PuzzleDir dir) => pathUseable[(int)dir];
    public void DisableTopPath() => topPath.gameObject.SetActive(false);
    public void DisableRightPath() => rightPath.gameObject.SetActive(false);
}
