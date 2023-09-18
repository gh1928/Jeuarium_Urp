using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANM_NextDoor : MonoBehaviour
{
    [SerializeField] Transform Basic_rotObj;

    [Header("RUNNING    ==========")]
    [SerializeField] float Baisc_rotX;

    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Unity           //////////
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.rotation.eulerAngles.x != Baisc_rotX)
        {
            Baisc_rotX = this.transform.rotation.eulerAngles.x;

            Basic_rotObj.localRotation = Quaternion.Euler(-Baisc_rotX, 0, 0);
        }
    }
}
