using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Deprecated
public class ClearDrawing : MonoBehaviour
{
    public GameObject clearObj;
    public GameObject board;

    public void Clear()
    {
        StartCoroutine(ClearDraw());
    }

    IEnumerator ClearDraw()
    {
        clearObj.transform.position = board.transform.position;
        clearObj.transform.rotation = board.transform.rotation;
        clearObj.transform.localScale = Vector3.Scale(board.transform.lossyScale, new Vector3(1, 1, -1));
        clearObj.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        clearObj.SetActive(false);
    }
}
