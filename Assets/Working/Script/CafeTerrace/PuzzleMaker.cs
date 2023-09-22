using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PuzzleMaker : MonoBehaviour
{    
    private int puzzleSize;
    public PuzzleNode nodePrefab;    
    public GameObject startPointPrefab;
    public GameObject endPointPrefab;
    public GameObject puzzleHolderPrafab;
    public float intervalAdjust;
    public PuzzleData Data;
    
    private NodeNumLinePair[] disconnectTarget;
    private PuzzleNode[,] puzzle;
    private int lastIdx;

    private RectTransform rect;
    private LookAtConstraint lookAtConstraints;
    private Transform puzzleHolder;

    private GameObject startPoint;
    private GameObject endPoint;

    private int currNodeNumber = 0;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        lookAtConstraints = GetComponent<LookAtConstraint>();
    }

    public void MakePuzzle()
    {
        lookAtConstraints.enabled = false;
        rect.localEulerAngles = Vector3.zero;

        puzzleHolder = Instantiate(puzzleHolderPrafab, transform).transform;

        ReadData();
        SetNodeAndLine();
        SetStartAndEndPoint();
        DisableNodeAndLine();

        lookAtConstraints.enabled = true;
    }
    public void ReadData()
    {
        puzzleSize = Data.PuzzleSize;
        lastIdx = puzzleSize - 1;
        puzzle = new PuzzleNode[puzzleSize, puzzleSize];
    }
    public void SetNodeAndLine()
    {
        float temp = (puzzleSize - 1) * intervalAdjust * 0.5f;
        Vector3 startPos = new Vector3(-temp, -temp, 0);

        for (int i = 0; i < puzzleSize; i++)
        {
            for (int j = 0; j < puzzleSize; j++)
            {
                Vector3 nodePos = new Vector3(j, i, 0) * intervalAdjust + startPos;
                var node = InstantiateNode(nodePos);
                node.Pos = (i,j);

                if (i == lastIdx)
                {
                    node.DisableTopPath();
                    node.SetPathable(PuzzleDir.Top, false);
                }

                if (j == lastIdx)
                {
                    node.DisableRightPath();
                    node.SetPathable(PuzzleDir.Right, false);
                }

                if (i == 0)
                    node.SetPathable(PuzzleDir.Bottom, false);

                if (j == 0)
                    node.SetPathable(PuzzleDir.Left, false);

                puzzle[i, j] = node;
            }
        }
    }
    private PuzzleNode InstantiateNode(Vector3 nodePos)
    {
        var node = Instantiate(nodePrefab, puzzleHolder);
        node.transform.localPosition = nodePos;
        node.SetPathAndLine(intervalAdjust);
        node.SetPlayerColor(Data.playerColor);
        node.SetColor();

        node.NodeNumber = currNodeNumber++;

        return node;
    }    
    private void SetStartAndEndPoint()
    {
        startPoint = Instantiate(startPointPrefab, puzzleHolder);
        startPoint.transform.position = puzzle[0, 0].transform.position;

        endPoint = Instantiate(endPointPrefab, puzzleHolder);
        endPoint.transform.position = puzzle[lastIdx, lastIdx].transform.position;
    }
    public void DisableNodeAndLine()
    {
        disconnectTarget = Data.disconnectTarget;

        foreach (var target in disconnectTarget)
        {
            if (target.nodeNumber >= puzzleSize * puzzleSize)
                continue;

            int posY = target.nodeNumber / puzzleSize;
            int posX = target.nodeNumber % puzzleSize;

            var targetNode = puzzle[posY, posX];

            targetNode.gameObject.SetActive(!target.offNode);
            
            if(target.offTop)
            {
                targetNode.DisableTopPath();

                targetNode.SetPathable(PuzzleDir.Top, false);

                if(posY < lastIdx)
                    puzzle[posY + 1, posX].SetPathable(PuzzleDir.Bottom, false);                
            }

            if(target.offRight)
            {
                targetNode.DisableRightPath();

                targetNode.SetPathable(PuzzleDir.Right, false);

                if(posX < lastIdx)
                    puzzle[posY, posX + 1].SetPathable(PuzzleDir.Left, false);
            }
        }
    }

    public void DestroyPuzzle() => Destroy(puzzleHolder.gameObject);
    public PuzzleNode[,] GetPuzzle() => puzzle;
    public GameObject GetStartPoint() => startPoint;
    public GameObject GetEndPoint() => endPoint;
    public Transform GetPuzzleHolder() => puzzleHolder;
}
