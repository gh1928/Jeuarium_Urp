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

[CreateAssetMenu(fileName = "PuzzleData", menuName = "Scriptable/PuzzleData")]
public class PuzzleData : ScriptableObject
{
    [Range(2, 5)]
    public int PuzzleSize;

    public NodeNumLinePair[] disconnectTarget;

    public Color baseColor;
    public Color playerColor;
}
