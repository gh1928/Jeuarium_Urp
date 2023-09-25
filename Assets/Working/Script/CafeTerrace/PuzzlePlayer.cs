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

    public float speed = 0.5f;

    public bool UseVR;    
    private PuzzleNode sourNode;
    private PuzzleNode destNode;

    private int exitNodeNum;
 
    private Stack<PuzzleNode> pathStack = new Stack<PuzzleNode>();

    private Vector2 lastInputPos;

    private int[] yDir = { 1, 0, -1, 0 };
    private int[] xDir = { 0, 1, 0, -1 };

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

        ActiveEnterPoint();

        sourNode = puzzleMaker.GetEnterNode();
        sourNode.OnVisitAction();

        exitNodeNum = puzzleMaker.GetExitNode().NodeNumber;

        Vector3 screenPointPosition = UseVR ?
            eventData.pointerCurrentRaycast.screenPosition : RectTransformUtility.WorldToScreenPoint(null, Input.mousePosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPointPosition, mainCam, out Vector2 localPointPosition);

        lastInputPos = localPointPosition;
    }
    public void ActiveEnterPoint()
    {
        var startPoint = puzzleMaker.GetEnterPoint();
        startPoint.GetComponent<RawImage>().color = puzzleMaker.Data.playerColor;
    }
    public void ActiveExitPoint()
    {
        var exitPoint = puzzleMaker.GetExitPoint();
        exitPoint.GetComponent<RawImage>().color= puzzleMaker.Data.playerColor;
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        Vector2 currInputPos = GetCurrInputPos();
        Vector2 inputMoveDelta = currInputPos - lastInputPos;

        lastInputPos = currInputPos;  

        if (sourNode.GetProgress() <= 0f)
        {
            SetDest(inputMoveDelta);
        }

        if (destNode == null)
            return;

        sourNode.UpdateLine(inputMoveDelta * speed);

        if (destNode.IsVisited())
            sourNode.ClampProgress(0.9f);            

        if (sourNode.GetProgress() >= 1f)
        {   
            ArriveDest();            
        }
    }
    private Vector2 GetCurrInputPos()
    {
        Vector3 screenPointPosition = UseVR ?
            eventData.pointerCurrentRaycast.screenPosition : RectTransformUtility.WorldToScreenPoint(null, Input.mousePosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPointPosition, mainCam, out Vector2 currInputPos);

        return currInputPos;
    }

    private void SetDest(Vector2 inputMoveDelta)
    {
        destNode = null;

        var dir = GetLargeDelta(inputMoveDelta);

        if (dir == PuzzleDir.None)
            return;        

        if (!sourNode.GetPathable(dir))
            return;

        if (pathStack.Count > 0 && dir == pathStack.Peek().GetOppositeDirection())
        {
            sourNode.CancleVisit();

            destNode = sourNode;
            sourNode = pathStack.Pop();

            return;
        } 

        int Idx = (int)dir;
        destNode = puzzle[sourNode.Pos.posY + yDir[Idx], sourNode.Pos.posX + xDir[Idx]];

        sourNode.SetDestDir(dir);        
    }

    private void ArriveDest()
    {
        destNode.OnVisitAction();

        if (destNode.NodeNumber == exitNodeNum)
        {
            ActiveExitPoint();

            isPlaying = false;
            return;
        }

        pathStack.Push(sourNode);
        sourNode = destNode;        
    }
    private PuzzleDir GetLargeDelta(Vector2 delta)
    {
        if (delta == Vector2.zero)
            return PuzzleDir.None;

        if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            return delta.x > 0 ? PuzzleDir.Right : PuzzleDir.Left;
        }

        return delta.y > 0 ? PuzzleDir.Top : PuzzleDir.Bottom;
    }

    public void StopPlay() => isPlaying = false;
}
