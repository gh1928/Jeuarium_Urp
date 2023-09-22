using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzlePlayer : MonoBehaviour
{
    private RectTransform rect;    
    private PuzzleMaker puzzleMaker;
    private PointerEventData eventData;
    private PuzzleNode[,] puzzle;
    private bool isPlaying = false;
    private Camera mainCam;

    public GameObject indicatorPrefab;
    private GameObject indicator;

    public float speed = 0.5f;

    public bool UseVR;    
    private PuzzleNode sourNode;
    private Image currLine;
    private PuzzleDir currLineDir;
    private PuzzleNode destNode;

    private Stack<PuzzleNode> pathStack = new Stack<PuzzleNode>();

    private Vector2 lastPointPos;

    private int[] yDir = { 1, 0, -1, 0 };
    private int[] xDir = { 0, 1, 0, -1 };

    private struct PathInfo
    {
        public PuzzleNode visitedNode;
        public Image lastLine;
        public PuzzleDir lastDir;
    }

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        puzzleMaker = GetComponent<PuzzleMaker>();

        if (UseVR)
            eventData = VRUISystem.Instance.EventData;

        mainCam = Camera.main;
    }
    public void StartPlay()
    {        
        puzzle = puzzleMaker.GetPuzzle();
        isPlaying = true;

        SetColorBeforePlay(puzzleMaker.Data.playerColor);

        sourNode = puzzle[0, 0];
        sourNode.OnVisited();
        pathStack.Push(sourNode);

        Vector3 screenPointPosition = UseVR ?
            eventData.pointerCurrentRaycast.screenPosition : RectTransformUtility.WorldToScreenPoint(null, Input.mousePosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPointPosition, mainCam, out Vector2 localPointPosition);

        lastPointPos = localPointPosition;
    }

    public void SetColorBeforePlay(Color playerColor)
    {
        var startPoint = puzzleMaker.GetStartPoint();
        startPoint.GetComponent<RawImage>().color = playerColor;

        indicator = Instantiate(indicatorPrefab, startPoint.transform.position, Quaternion.Euler(startPoint.transform.localEulerAngles), puzzleMaker.GetPuzzleHolder());
        indicator.GetComponent<Image>().color = playerColor;
    }
    private void Update()
    {
        if (!isPlaying)
            return;

        Vector3 screenPointPosition = UseVR ?
            eventData.pointerCurrentRaycast.screenPosition : RectTransformUtility.WorldToScreenPoint(null, Input.mousePosition);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPointPosition, mainCam, out Vector2 localPointPosition);

        Vector2 pointDelta = localPointPosition - lastPointPos;
        lastPointPos = localPointPosition;

        if (destNode == null)
        {
            var dir = GetMaxDeltaDir(pointDelta);
            if (dir == PuzzleDir.None)
                return;

            if(sourNode.GetPathable(dir))
            {
                int dirIdx = (int)dir;
                destNode = puzzle[sourNode.Pos.posY + yDir[dirIdx], sourNode.Pos.posX + xDir[dirIdx]];
                currLineDir = dir;
                currLine = sourNode.GetDirLine(dir);
            }
            return;
        }

        float lineSizeChanger = 0;
        switch (currLineDir)
        {
            case PuzzleDir.Top:
                lineSizeChanger = pointDelta.y;
                break;
            case PuzzleDir.Right:
                lineSizeChanger = pointDelta.x;
                break;
            case PuzzleDir.Bottom:
                lineSizeChanger = - pointDelta.y;
                break;
            case PuzzleDir.Left:
                lineSizeChanger = - pointDelta.x;
                break;
        }

        currLine.fillAmount += lineSizeChanger * speed;

        if (destNode.IsVisited() && currLine.fillAmount > 0.9f)
            currLine.fillAmount = 0.9f;

        if(currLine.fillAmount <= 0)
        {
            destNode = null;

            if(pathStack.Count > 0)
            {
                destNode = pathStack.Pop();
                sourNode = pathStack.Peek();
            }
        }    

        if(currLine.fillAmount >= 1)
        {
            pathStack.Push(destNode);
            destNode.OnVisited();
            sourNode = destNode;
            destNode = null;
        }

        //Debug.Log(currLine.fillAmount);        
    }

    private PuzzleDir GetMaxDeltaDir(Vector2 delta)
    {
        if (delta == Vector2.zero)
            return PuzzleDir.None;

        if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            return delta.x > 0 ? PuzzleDir.Right : PuzzleDir.Left;
        }

        return delta.y > 0 ? PuzzleDir.Top : PuzzleDir.Bottom;
    }
}
