using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANM_GoghYellowhouse_Tube : MonoBehaviour
{
    [SerializeField] Transform Basic_parent;
    [SerializeField] GameObject Basic_cap;
    [SerializeField] ANM_GoghYellowhouse_Paint Basic_paint;

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

    //
    private void OnTriggerStay(Collider _other)
    {
        if(_other.gameObject.Equals(Basic_paint.gameObject))
        {
            if (Basic_paint.enabled)
            {
                if (this.transform.position.y < Basic_parent.transform.position.y)
                {
                    Basic_cap.SetActive(false);

                    Basic_paint.ANM_Basic_Tube();
                }
            }
        }
    }

    //
    private void OnTriggerExit(Collider _other)
    {
        if (_other.gameObject.Equals(Basic_paint.gameObject))
        {
            Basic_cap.SetActive(true);
        }
    }
}
