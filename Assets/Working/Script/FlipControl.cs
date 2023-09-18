using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode]
public class FlipControl : MonoBehaviour
{
    public GameObject[] InvertList;
    void Update()
    {
        if (InvertList.Length<1) return;
        for (int i = 0; i < InvertList.Length; i++)
        {
            InvertList[i].AddComponent<MeshFlip>();
        }
        InvertList = null;
    }

}
