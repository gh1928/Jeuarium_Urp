using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTest : MonoBehaviour
{
    public void NotifyPointerClick()
    {
        Debug.Log("Pointer Clicked!");
    }

    public void GetHeight()
    {
        //Debug.Log(transform.localScale.y);

        var height = GetComponent<RectTransform>().sizeDelta.y;

        Debug.Log(height);

        GetComponent<RectTransform>().sizeDelta = Vector2.one;
    }
  
}
