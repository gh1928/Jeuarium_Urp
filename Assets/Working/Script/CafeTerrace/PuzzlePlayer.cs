using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzlePlayer : MonoBehaviour
{   
    private PointerEventData eventData;
    
    private PuzzleMaker puzzleMaker;    
    private PuzzleNode[,] puzzle;
    private bool isPlaying = false;    

    public float speed = 0.5f;

    public bool UseVR;    
    private PuzzleNode currNode;
    private PuzzleNode destNode;

    private int exitNodeNum;
 
    private Stack<PuzzleNode> pathStack = new Stack<PuzzleNode>();

    private Vector2 lastInputPos;

    private int[] yDir = { 1, 0, -1, 0 };
    private int[] xDir = { 0, 1, 0, -1 };

    private void Start()
    {   
        puzzleMaker = GetComponent<PuzzleMaker>();

        if (UseVR)
            eventData = VRUISystem.Instance.EventData;
    }
    public void StartPlay()
    {        
        puzzle = puzzleMaker.GetPuzzle();
        isPlaying = true;

        ActivePoint(puzzleMaker.GetEnterPoint());

        currNode = puzzleMaker.GetEnterNode();
        currNode.OnVisitAction();

        exitNodeNum = puzzleMaker.GetExitNode().NodeNumber;

        lastInputPos = eventData.pointerCurrentRaycast.worldPosition;
    }
    public void ActivePoint(RawImage point) => point.color = puzzleMaker.Data.playerColor;

    private void Update()
    {
        if (!isPlaying)
            return;

        if (eventData.pointerCurrentRaycast.module.GetType() != typeof(GraphicRaycaster))
            return;
        
        Vector2 currInputPos = eventData.pointerCurrentRaycast.worldPosition;
        Vector2 inputDelta = currInputPos - lastInputPos;        
        


        lastInputPos = currInputPos;

        if (currNode.GetProgress() <= 0f)
            SetDest(inputDelta);

        if (destNode == null)
            return;

        currNode.UpdateLine(inputDelta * speed);

        if (destNode.IsVisited())
            currNode.ClampProgress(0.9f);            

        if (currNode.GetProgress() >= 1f)
            VisitDest();
    }

    private void SetDest(Vector2 inputMoveDelta)
    {
        destNode = null;

        var dir = GetLargeDelta(inputMoveDelta);

        if (dir == PuzzleDir.None)
            return;        

        if (!currNode.GetPathable(dir))
            return;

        if (pathStack.Count > 0 && dir == pathStack.Peek().GetOppositeDirection())
        {
            currNode.CancleVisit();

            destNode = currNode;
            currNode = pathStack.Pop();

            return;
        } 

        int Idx = (int)dir;
        destNode = puzzle[currNode.Pos.posY + yDir[Idx], currNode.Pos.posX + xDir[Idx]];

        currNode.SetDestDir(dir);        
    }

    private void VisitDest()
    {
        destNode.OnVisitAction();

        if (destNode.NodeNumber == exitNodeNum)
        {
            ActivePoint(puzzleMaker.GetExitPoint());

            isPlaying = false;
            return;
        }

        pathStack.Push(currNode);
        currNode = destNode;        
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
