using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NodeNumLinePair
{
    public int nodeNumber;
    public bool offNode;
    public bool offTop;
    public bool offRight;
}
public class PuzzleMaker : MonoBehaviour
{
    [Range(2, 5)]
    public int PuzzleSize;
    public PuzzleNode nodePrefab;    
    public GameObject startPointPrefab;
    public GameObject endPointPrefab;
    public float intervalAdjust;
    
    public NodeNumLinePair[] disconnectTarget;

    private PuzzleNode[,] puzzle;
    private int lastIdx;

    public void MakePuzzle()
    {
        SetNodeAndLine();
        SetStartAndEndPoint();
        DisableNodeAndLine();
    }
    public void SetNodeAndLine()
    {
        float temp = (PuzzleSize - 1) * intervalAdjust * 0.5f;
        Vector3 startPos = new Vector3(-temp, -temp, 0);

        puzzle = new PuzzleNode[PuzzleSize, PuzzleSize];
        lastIdx = PuzzleSize - 1;

        for (int i = 0; i < PuzzleSize; i++)
        {
            for (int j = 0; j < PuzzleSize; j++)
            {
                Vector3 nodePos = new Vector3(j, i, 0) * intervalAdjust + startPos;

                var node = Instantiate(nodePrefab, transform);
                node.transform.localPosition = nodePos;
                puzzle[i, j] = node;

                node.SetLine(intervalAdjust);

                if (i == lastIdx)
                    node.DisableTopLine();

                if (j == lastIdx)
                    node.DisableRightLine();
            }
        }
    }
    private void SetStartAndEndPoint()
    {
        var startPoint = Instantiate(startPointPrefab, transform);
        startPoint.transform.position = puzzle[0, 0].transform.position;

        var endPoint = Instantiate(endPointPrefab, transform);
        endPoint.transform.position = puzzle[lastIdx, lastIdx].transform.position;
    }
    public void DisableNodeAndLine()
    {
        foreach(var target in disconnectTarget)
        {
            if (target.nodeNumber >= PuzzleSize * PuzzleSize)
                continue;

            var targetNode = puzzle[target.nodeNumber / PuzzleSize, target.nodeNumber % PuzzleSize];

            if(target.offNode)
                targetNode.gameObject.SetActive(false);

            targetNode.DisableLine(target.offTop, target.offRight);
        }
    }
}
