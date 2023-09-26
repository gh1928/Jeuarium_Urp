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
    
    private PuzzleNode currNode;
    private PuzzleNode destNode;
    private Transform indicator;

    private int enterNodeNum;
    private int exitNodeNum;
 
    private Stack<PuzzleNode> pathStack = new Stack<PuzzleNode>();

    private int[] yDir = { 1, 0, -1, 0 };
    private int[] xDir = { 0, 1, 0, -1 };

    private float clampValue;
    private float enterClampValue;

    public float minDistanceToMove = 1f;
    public float minDistanceToExitNode = 1f;

    private void Start()
    {   
        puzzleMaker = GetComponent<PuzzleMaker>();
        eventData = VRUISystem.Instance.EventData;
    }
    public void StartPlay()
    {        
        puzzle = puzzleMaker.GetPuzzle();

        SetClampValue();
        ActivePoint(puzzleMaker.GetEnterPoint());
        indicator = puzzleMaker.GetIndicator().transform;
        currNode = puzzleMaker.GetEnterNode();
        enterNodeNum = currNode.NodeNumber;
        exitNodeNum = puzzleMaker.GetExitNode().NodeNumber;

        currNode.OnVisitAction();

        isPlaying = true;
    }
    private void SetClampValue()
    {
        float interval = puzzleMaker.Data.nodeInterval;

        clampValue = (interval - 0.15f) / interval;

        enterClampValue = (interval - 0.225f) / interval;
    }
    public void ActivePoint(RawImage point) => point.color = puzzleMaker.Data.playerColor;

    private void Update()
    {
        if (!isPlaying)
            return;

        if (eventData.pointerCurrentRaycast.gameObject == null)
            return;

        var currTarget = eventData.pointerCurrentRaycast.module;

        if (currTarget == null || currTarget.GetType() != typeof(GraphicRaycaster))
            return;

        Vector3 currInputPos = eventData.pointerCurrentRaycast.worldPosition;            

        Vector3 pointerDir = currInputPos - indicator.position;

        if (Vector3.SqrMagnitude(pointerDir) < minDistanceToMove)
            return;

        var puzzleDir = GetLargeDirValue(pointerDir);
        float currProgress = currNode.GetProgress();

        if (currProgress <= 0f)
        {
            if (Vector3.SqrMagnitude(pointerDir) < minDistanceToExitNode)
                return;

            SetDest(puzzleDir);
        }

        if (destNode == null)
            return;

        //currNode.UpdateLine(pointerDir, speed * Time.deltaTime);

        currNode.UpdateLine(speed * Time.deltaTime * pointerDir);

        indicator.transform.position = Vector3.Lerp(currNode.transform.position, destNode.transform.position, currProgress);

        if (destNode.IsVisited())
            currNode.ClampProgress(clampValue);

        if(destNode.NodeNumber == enterNodeNum)
            currNode.ClampProgress(enterClampValue);

        if (currProgress >= 1f)
            VisitDest();
    }

    private void SetDest(PuzzleDir dir)
    {
        destNode = null;

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
    private PuzzleDir GetLargeDirValue(Vector2 look)
    {
        if (look == Vector2.zero)
            return PuzzleDir.None;

        if(Mathf.Abs(look.x) > Mathf.Abs(look.y))
        {
            return look.x > 0 ? PuzzleDir.Left : PuzzleDir.Right;
        }

        return look.y > 0 ? PuzzleDir.Top : PuzzleDir.Bottom;
    }
    public void StopPlay() => isPlaying = false;
}
