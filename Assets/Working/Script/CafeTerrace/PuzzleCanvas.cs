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

        // Raycast�� �����Ͽ� Ŭ���� UI ��Ҹ� ã���ϴ�.
        var results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        // Ŭ���� UI ��Ұ� �ִٸ� ����� ����մϴ�.
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
