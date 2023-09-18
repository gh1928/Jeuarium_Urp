using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NonPlayableCharacterManager : MonoBehaviour
{
    public static NonPlayableCharacterManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }

    public static Vector3 GetRandomPointOnInstanceRange()
    {
        Vector3 randomPos = instance.transform.TransformPoint(new Vector3(Random.Range(-0.5f,0.5f),
            Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)));

        if(NavMesh.SamplePosition(randomPos, out NavMeshHit hit, instance.transform.localScale.y, 1))
        {
            return hit.position;
        }
        else
        {
            if ( NavMesh.FindClosestEdge(randomPos, out hit, 1))
            {
                return hit.position;
            }
        }
        return instance.transform.position;
    }
}
