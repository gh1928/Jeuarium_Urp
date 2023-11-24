using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANM_GoghYellowhouse_Knife : MonoBehaviour
{
    [SerializeField] ANM_GoghYellowhouse_Manager Basic_Manager;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////

    ////////// Unity            //////////
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider _other)
    {
        switch (_other.tag)
        {
            case "Paint":
                {
                    if (!_other.GetComponent<ANM_GoghYellowhouse_Paint>().enabled)
                    {
                        MeshRenderer mr = _other.GetComponent<MeshRenderer>();
                        switch (mr.material.name)
                        {
                            case "Paint_Mix (Instance)":
                                {
                                    Basic_Manager.ANM_Knife_Mix();
                                }
                                break;
                            default:
                                {
                                    Basic_Manager.ANM_Knife_SelectMat(mr.material);
                                }
                                break;
                        }
                    }
                }
                break;
        }
    }
}
