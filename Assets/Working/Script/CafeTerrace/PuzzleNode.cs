using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleNode : MonoBehaviour
{
    private RectTransform rect;

    public RectTransform topLine;
    public RectTransform rightLine;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    public void SetLine(float interval)
    {
        float currSize = rect.sizeDelta.y;

        topLine.transform.position = transform.position + 0.5f * interval * Vector3.up;        
        topLine.sizeDelta = new Vector2 (currSize, interval);

        rightLine.transform.position = transform.position + 0.5f * interval * Vector3.right;
        rightLine.sizeDelta = new Vector2 (interval, currSize);
    }
    public void DisableLine(bool offTop, bool offRight)
    {
        if (offTop) 
            DisableTopLine();
        if (offRight) 
            DisableRightLine();
    }

    public void DisableTopLine() => topLine.gameObject.SetActive(false);
    public void DisableRightLine() => rightLine.gameObject.SetActive(false);

}
