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
                    Basic_Manager.ANM_Brush_selectPaint = _other.GetComponent<PaintBarrelTrigger>();
                    Color color = _other.GetComponent<PaintBarrelTrigger>().list_Mat[1].GetColor("_BaseColor");
                    this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", color);
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
