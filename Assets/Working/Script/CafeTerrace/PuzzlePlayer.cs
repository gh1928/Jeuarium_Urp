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

        SetColorBeforePlay(puzzleMaker.Data.playerColor);

        sourNode = puzzle[0, 0];
        sourNode.OnVisitAction();
        pathStack.Push(sourNode);

        Vector3 screenPointPosition = UseVR ?
            eventData.pointerCurrentRaycast.screenPosition : RectTransformUtility.WorldToScreenPoint(null, Input.mousePosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPointPosition, mainCam, out Vector2 localPointPosition);

        lastInputPos = localPointPosition;
    }
    public void SetColorBeforePlay(Color playerColor)
    {
        var startPoint = puzzleMaker.GetStartPoint();
        startPoint.GetComponent<RawImage>().color = playerColor;
    }
    private void Update()
    {
        if (!isPlaying)
            return;

        Vector2 currInputPos = GetCurrInputPos();
        Vector2 inputMoveDelta = currInputPos - lastInputPos;

        lastInputPos = currInputPos;

        if(destNode == null)
        {
            SetDest(inputMoveDelta);
            return;
        }

        sourNode.UpdateLine(inputMoveDelta * speed, destNode.IsVisited());

        if(sourNode.GetProgress() <= 0)
        {
            destNode = null;

            if(pathStack.Count >= 2)
            {
                destNode = pathStack.Pop();
                sourNode = pathStack.Peek();
            }
        }    

        if(sourNode.GetProgress() >= 1)
        {
            pathStack.Push(destNode);
            sourNode = destNode;
            destNode.OnVisitAction();
            destNode = null;
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
        var dir = GetLargeDelta(inputMoveDelta);

        if (dir == PuzzleDir.None)
            return;

        if (!sourNode.GetPathable(dir))
            return;

        int Idx = (int)dir;
        destNode = puzzle[sourNode.Pos.posY + yDir[Idx], sourNode.Pos.posX + xDir[Idx]];

        sourNode.SetDestDir(dir);
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
}
