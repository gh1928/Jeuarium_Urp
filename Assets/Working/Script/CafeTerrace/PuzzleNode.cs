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
    private bool[] pathUseable = { true, true, true, true };

    private bool visited = false;

    private Color playerColor;

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
    public Image GetDirLine(PuzzleDir dir) => lines[(int)dir];

    public void SetPathAndLine(float interval)
    {
        float currSize = rect.sizeDelta.y;

        topPath.transform.position = transform.position + 0.5f * interval * Vector3.up;        
        topPath.sizeDelta = new Vector2 (currSize, interval);

        rightPath.transform.position = transform.position + 0.5f * interval * Vector3.right;
        rightPath.sizeDelta = new Vector2 (interval, currSize);

        Vector3[] dirs = new Vector3[4];
        dirs[0] = Vector3.up;
        dirs[1] = Vector3.right; 
        dirs[2] = Vector3.down;
        dirs[3] = Vector3.left;

        for(int i = 0; i < 4;i++)
        {
            bool isVertical = i % 2 == 0;

            lines[i].transform.position = transform.position + 0.5f * interval * dirs[i];
            lines[i].rectTransform.sizeDelta = new Vector2(isVertical ? currSize : interval, isVertical ? interval : currSize);
        }
    }
    public void OnVisited()
    {
        visited = true;
        nodeImage.color = playerColor;
    }
    public bool IsVisited() => visited;
    public void SetPathable(PuzzleDir dir, bool pathable) => pathUseable[(int)dir] = pathable;
    public bool GetPathable(PuzzleDir dir) => pathUseable[(int)dir];
    public void DisableTopPath() => topPath.gameObject.SetActive(false);
    public void DisableRightPath() => rightPath.gameObject.SetActive(false);
}
