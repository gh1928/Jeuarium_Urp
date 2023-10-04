using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NodeNumLinePair
{
    public int nodeNumber;
    public bool offNode;
    public bool offTopPath;
    public bool offRightPath;
}

[System.Serializable]
public struct PosPair
{
    public int y;
    public int x;
}

[System.Serializable]
public struct ElementInfo
{
    public Elements elements;
    public int nodeNumber;
    
    public bool placeAtNode;
    public bool placeAtTop;
    public bool placeAtRight;
}

[CreateAssetMenu(fileName = "PuzzleData", menuName = "Scriptable/PuzzleData")]
public class PuzzleData : ScriptableObject
{
    [Range(2, 5)]
    public int PuzzleSize;

    public NodeNumLinePair[] puzzleInfo;
    public ElementInfo[] elementsInfo;

    public PosPair enterPos;
    public PosPair exitPos;    

    public Color baseColor;
    public Color playerColor;

    public float nodeInterval = 1f;

    public int eventIdx;
}
