using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleCanvas : MonoBehaviour
{
    //2 * n * (n - 1)
    
    public GameObject testCube;

    PointerEventData pointerEventData;
    private EventSystem eventSystem;
    GraphicRaycaster raycaster;
    public void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();        
        pointerEventData = new PointerEventData(eventSystem);
    }
    public void MouseTest()
    {
        pointerEventData.position = Input.mousePosition;

        // Raycast를 실행하여 클릭된 UI 요소를 찾습니다.
        var results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        // 클릭된 UI 요소가 있다면 결과를 출력합니다.
        foreach (var result in results)
        {
            Debug.Log(result.worldPosition);

            
            testCube.transform.position = result.worldPosition;
        }
    }
    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("ButtonClicked");
        }
    }
}
