using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistBone : MonoBehaviour
{
    [SerializeField] private Transform sourceBone;

    [SerializeField, Range(0,1)] private float weight;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            transform.rotation = Quaternion.LookRotation(
                Vector3.Cross(transform.parent.up, Vector3.Slerp(-transform.parent.right, -sourceBone.right, weight))
                ,transform.parent.up);
        }
    }

    
}
