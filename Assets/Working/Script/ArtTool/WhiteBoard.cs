using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBoard : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(SetWhiteBoardLayer());
    }

    IEnumerator SetWhiteBoardLayer()
    {
        this.gameObject.layer = 11;
        yield return new WaitForSeconds(5f);
        this.gameObject.layer = 0;
    }
}
