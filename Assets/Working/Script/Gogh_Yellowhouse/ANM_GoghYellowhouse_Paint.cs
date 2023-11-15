using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANM_GoghYellowhouse_Paint : MonoBehaviour
{
    [SerializeField] ANM_Manager Basic_Manager;

    [SerializeField] Transform Basic_origin;
    [SerializeField] float Basic_valueTime;

    [Header("RUNNING")]
    [SerializeField] Vector3 Basic_position;
    [SerializeField] Vector3 Basic_localScale;
    [SerializeField] float Basic_value;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public void ANM_Basic_Tube()
    {
        Basic_value += Time.deltaTime;

        if(Basic_value >= Basic_valueTime)
        {
            Basic_value = Basic_valueTime;

            //
            if (Basic_Manager != null)
            {
                Basic_Manager.ANM_Event_Trigger(this.gameObject);
            }
            this.enabled = false;
        }

        //
        this.transform.localPosition    = Vector3.Lerp( Basic_position,     Basic_origin.localPosition, Basic_value / Basic_valueTime);
        this.transform.localScale       = Vector3.Lerp( Basic_localScale,   Basic_origin.localScale,    Basic_value / Basic_valueTime);
    }

    ////////// Unity            //////////
    // Start is called before the first frame update
    void Start()
    {
        Basic_position = this.transform.localPosition;
        Basic_localScale = this.transform.localScale;
        Basic_value = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
