using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANM_GoghYellowhouse_Brush : MonoBehaviour
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
                        Color c = _other.gameObject.GetComponent<MeshRenderer>().material.GetColor("_BaseColor");
                        Basic_Manager.ANM_Brush_Select(c);
                        this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", c);
                    }
                }
                break;
            case "Canvas":
                {
                    Basic_Manager.ANM_Brush_Painting();
                }
                break;
        }
    }
}
