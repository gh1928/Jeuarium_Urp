using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrushRangeObj : MonoBehaviour
{
    CapsuleCollider rangeCollider;
    public GameObject patternPrefab;
    Transform root;

    void Start()
    {
        rangeCollider = GetComponent<CapsuleCollider>();
        //CreatePatternPrefab(DataManager.instance.BrushCount);
    }

    public void CreatePatternPrefab(int count)
    {
        root = new GameObject().transform;
        root.name = "BrushPatternHolder";
        root.parent = transform;

        for (int i = 0; i < count; i++)
        {
            GameObject pattern = Instantiate(patternPrefab, RandomPosition(), transform.rotation);
            //pattern.GetComponent<MeshRenderer>().material.color = DataManager.instance.BrushColor;
            //pattern.transform.localScale *= DataManager.instance.BrushSize;
            pattern.transform.parent = root;
        }
    }

    Vector3 RandomPosition()
    {
        Vector3 RandomPostion = Random.onUnitSphere;
        RandomPostion.y = 0.001f;

        float range_X = rangeCollider.bounds.size.x * 0.45f;
        float radius = Random.Range(0f, range_X);

        Vector3 respawnPosition = transform.position + RandomPostion * radius;
        return respawnPosition;
    }
}
