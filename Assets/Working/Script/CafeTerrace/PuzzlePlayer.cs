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

    public GameObject indicatorPrefab;
    private GameObject indicator;

    public bool UseVR;    

    private PuzzleNode currNode;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        puzzleMaker = GetComponent<PuzzleMaker>();

        if (UseVR)
            eventData = VRUISystem.Instance.EventData;
    }
    public void StartPlay()
    {        
        puzzle = puzzleMaker.GetPuzzle();
        isPlaying = true;

        Color playerColor = puzzleMaker.Data.playerColor;

        var startPoint = puzzleMaker.GetStartPoint();
        startPoint.GetComponent<RawImage>().color = playerColor;

        indicator = Instantiate(indicatorPrefab, startPoint.transform.position, Quaternion.Euler(startPoint.transform.localEulerAngles), puzzleMaker.GetPuzzleHolder());
        indicator.GetComponent<Image>().color = playerColor;

    }
    private void Update()
    {
        if (!isPlaying)
            return;

        Vector3 screenPointPosition = UseVR ?
            eventData.pointerCurrentRaycast.screenPosition : RectTransformUtility.WorldToScreenPoint(null, Input.mousePosition);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPointPosition, Camera.main, out Vector2 localPointPosition);

        //Debug.Log(localPointPosition);        
    }
}
