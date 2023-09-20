using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzlePlay : MonoBehaviour
{
    public GameObject checkPoint;
    public Image line;

    private PuzzleNode[,] puzzle;
    public void SetPuzzle(PuzzleNode[,] puzzle) => this.puzzle = puzzle;    
    public void StartPuzzle()
    {

    }   
}
