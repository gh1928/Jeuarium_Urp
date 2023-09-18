using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScriptForActionTriggerManagerScene : MonoBehaviour
{
    public GameObject targetGO;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            targetGO.SetActive(!targetGO.activeInHierarchy);
        }
    }
}
