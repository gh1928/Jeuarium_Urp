using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class PuzzleMaker : MonoBehaviour
{    
    private int puzzleSize;
    public PuzzleNode nodePrefab;    
    public RawImage entAndExitPrefab;

    private GameObject indicator;
    public GameObject indicatorPrefab;
    
    public Transform puzzleHolderPrafab;
    private float intervalAdjust;

    public PuzzleData[] puzzleDatas;
    public PuzzleData CurrData { get; set; }
    private int currStep = 0;

    private NodeNumLinePair[] puzzleInfo;
    private PuzzleNode[,] puzzle;
    private int lastIdx;

    private RectTransform rect;
    private LookAtConstraint lookAtConstraints;
    private Transform puzzleHolder;

    private RawImage enterPoint;
    private RawImage exitPoint;

    private int currNodeNumber = 0;

    public PuzzleElement[] elementPrefabs;
    private List<PuzzleElement> instancedElements = new List<PuzzleElement>();

    private Vector3 scale;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        scale = rect.localScale;

        lookAtConstraints = GetComponent<LookAtConstraint>();
        CurrData = puzzleDatas[currStep];
    }

    private void Start()
    {
        MakePuzzle();
    }

    public void MakePuzzle()
    {
        rect.localScale = Vector3.one;
        lookAtConstraints.enabled = false;
        rect.localEulerAngles = Vector3.zero;

        puzzleHolder = Instantiate(puzzleHolderPrafab, transform);

        MakePuzzleArray();
        SetNodeAndLine();
        SetEnterAndExitPoint();
        ReadPuzzleInfo();
        ReadElemtnsInfo();

        rect.localScale = scale;
        lookAtConstraints.enabled = true;
    }
    public void StopPuzzle()
    {
        GetComponent<PuzzlePlayer>().StopPlay();
        Destroy(puzzleHolder.gameObject);
    }
    public void ResetPuzzle()
    {
        currNodeNumber = 0;

        StopPuzzle();
        MakePuzzle();
    }
    private void MakePuzzleArray()
    {        
        puzzleSize = CurrData.PuzzleSize;
        lastIdx = puzzleSize - 1;
        puzzle = new PuzzleNode[puzzleSize, puzzleSize];
    }
    private void SetNodeAndLine()
    {
        intervalAdjust = CurrData.nodeInterval;

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
                    node.SetPathable(PuzzleDir.Up, false);
                }

                if (j == lastIdx)
                {
                    node.DisableRightPath();
                    node.SetPathable(PuzzleDir.Right, false);
                }

                if (i == 0)
                    node.SetPathable(PuzzleDir.Down, false);

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
        node.SetPathAndProgressLine(intervalAdjust);
        node.SetPlayerColor(CurrData.playerColor);
        node.SetBaseColor(CurrData.baseColor);
        node.SetColor();

        node.NodeNumber = currNodeNumber++;

        return node;
    }    
    private void SetEnterAndExitPoint()
    {
        var enterPos = CurrData.enterPos;
        enterPoint = Instantiate(entAndExitPrefab, puzzleHolder);
        enterPoint.transform.position = puzzle[enterPos.y, enterPos.x].transform.position;
        enterPoint.color = CurrData.playerColor;

        indicator = Instantiate(indicatorPrefab, puzzleHolder);
        indicator.transform.position = enterPoint.transform.position;
        indicator.GetComponent<Image>().color = CurrData.playerColor;

        var exitPos = CurrData.exitPos;
        exitPoint = Instantiate(entAndExitPrefab, puzzleHolder);
        exitPoint.transform.position = puzzle[exitPos.y, exitPos.x].transform.position;
        exitPoint.color = CurrData.baseColor;
    }
    public void ReadPuzzleInfo()
    {
        puzzleInfo = CurrData.puzzleInfo;

        foreach (var target in puzzleInfo)
        {
            if (target.nodeNumber >= puzzleSize * puzzleSize)
                continue;

            int posY = target.nodeNumber / puzzleSize;
            int posX = target.nodeNumber % puzzleSize;

            var targetNode = puzzle[posY, posX];

            targetNode.gameObject.SetActive(!target.offNode);
            
            if(target.offTopPath)
            {
                targetNode.DisableTopPath();

                targetNode.SetPathable(PuzzleDir.Up, false);

                if(posY < lastIdx)
                    puzzle[posY + 1, posX].SetPathable(PuzzleDir.Down, false);                
            }

            if(target.offRightPath)
            {
                targetNode.DisableRightPath();

                targetNode.SetPathable(PuzzleDir.Right, false);

                if(posX < lastIdx)
                    puzzle[posY, posX + 1].SetPathable(PuzzleDir.Left, false);
            }
        }
    }

    public void ReadElemtnsInfo()
    {
        var elementsInfo = CurrData.elementsInfo;
        instancedElements.Clear();

        foreach (var info in elementsInfo)
        {
            if (info.nodeNumber >= puzzleSize * puzzleSize)
                continue;

            int posY = info.nodeNumber / puzzleSize;
            int posX = info.nodeNumber % puzzleSize;

            Vector3 elementPos = Vector3.zero;
            Vector3 nodePos = puzzle[posY, posX].transform.position;

            if (info.placeAtNode)
                elementPos = nodePos;

            if (info.placeAtTop)
                elementPos = Vector3.Lerp(nodePos, puzzle[posY + 1, posX].transform.position, 0.5f);

            if(info.placeAtRight)
                elementPos = Vector3.Lerp(nodePos, puzzle[posY, posX + 1].transform.position, 0.5f);

            var element = Instantiate(elementPrefabs[(int)info.elements], elementPos, Quaternion.identity, puzzleHolder);

            if (info.placeAtNode)
                element.OnPlaceAtNode(puzzle[posY, posX]);

            instancedElements.Add(element);
        }
    }
    public void SetNextStep()
    {
        CurrData = puzzleDatas[++currStep];
        ResetPuzzle();
    }
    public bool IsRemainPuzzle() => currStep < puzzleDatas.Length - 1;
    public void DestroyPuzzle() => Destroy(puzzleHolder.gameObject);
    public PuzzleNode[,] GetPuzzle() => puzzle;
    public List<PuzzleElement> GetInstancedElements() => instancedElements;
    public RawImage GetEnterPoint() => enterPoint;
    public RawImage GetExitPoint() => exitPoint;
    public Transform GetPuzzleHolder() => puzzleHolder;
    public PuzzleNode GetEnterNode() => puzzle[CurrData.enterPos.y,CurrData.enterPos.x];
    public PuzzleNode GetExitNode() => puzzle[CurrData.exitPos.y, CurrData.exitPos.y];
    public GameObject GetIndicator()=> indicator;
}
