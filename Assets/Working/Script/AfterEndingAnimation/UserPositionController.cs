using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPositionController : MonoBehaviour
{
    public Transform user;
    public void OnChangeUserPos(Transform pivot)
    {
        user.position = pivot.position;
        user.localEulerAngles = pivot.localEulerAngles;
    }
}
