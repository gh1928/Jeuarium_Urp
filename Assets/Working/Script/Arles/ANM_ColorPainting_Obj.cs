using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using BNG;

public class ANM_ColorPainting_Obj : GrabbableEvents
{
    public enum PHASE
    {
        NONE,
        //
        TRANSFORM__INIT,
        TRANSFORM,
        TRANSFORM__END,
        //
        ANIMATION__START,
        AMIMATION__END_CHECK,
        AMIMATION__END
    }

    [SerializeField] ANM_ColorPainting Basic_ColorPaint;
    [SerializeField] ANM_Manager Basic_Manager;
    [SerializeField] List<GameObject> Basic_objs;
    [SerializeField] List<string> Basic_strs;
    [SerializeField] List<float> Basic_floats;

    [SerializeField] Animator Basic_animator;

    [Header("RUNNING")]
    [SerializeField] PHASE Basic_phase;
    [SerializeField] List<Vector3> Basic_vec3s;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    // OnGrab
    public override void OnGrab(BNG.Grabber _grabber)
    {
        switch(this.tag)
        {
            case "Ball":    { OnGrab_Ball();    }   break;
            case "Paint":   { ObGrab_Paint();   }   break;
        }

        base.OnGrab(_grabber);
    }

    void OnGrab_Ball()
    {
        Basic_Manager.ANM_Hand_GrabRelease(this.gameObject);
        GetComponent<Grabbable>().enabled = false;

        //
        Basic_phase = PHASE.ANIMATION__START;
    }

    void ObGrab_Paint()
    {
        Basic_Manager.ANM_Hand_GrabRelease(this.gameObject);
        GetComponent<Grabbable>().enabled = false;

        //
        Basic_phase = PHASE.TRANSFORM__INIT;
    }

    ////////// Unity            //////////
    // Start is called before the first frame update
    void Start()
    {
        Basic_phase = PHASE.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        switch (Basic_phase)
        {
            case PHASE.TRANSFORM__INIT: { ANM_Update_TRANSFORM__INIT(); }   break;
            case PHASE.TRANSFORM:       { ANM_Update_TRANSFORM();       }   break;

            //
            case PHASE.ANIMATION__START:        { ANM_Update_ANIMATION__START();        }   break;
            case PHASE.AMIMATION__END_CHECK:    { ANM_Update_AMIMATION__END_CHECK();    }   break;
        }
    }

    //
    void ANM_Update_TRANSFORM__INIT()
    {
        if (Basic_vec3s == null)
        {
            Basic_vec3s = new List<Vector3>();
        }
        Basic_vec3s.Add(new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z));
        Basic_vec3s.Add(new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z));

        Basic_floats[0] = 0.0f;

        //
        Basic_phase = PHASE.TRANSFORM;
    }

    void ANM_Update_TRANSFORM()
    {
        Basic_floats[0] += Time.deltaTime;
        if (Basic_floats[0] >= Basic_floats[1])
        {
            Basic_floats[0] = Basic_floats[1];
            //
            Basic_phase = PHASE.TRANSFORM__END;
        }

        this.transform.position = Vector3.Lerp(Basic_vec3s[0], Basic_objs[0].transform.position, Basic_floats[0] / Basic_floats[1]);
        this.transform.rotation = Quaternion.Euler(Vector3.Lerp(Basic_vec3s[1], Basic_objs[0].transform.rotation.eulerAngles, Basic_floats[0] / Basic_floats[1]));
    }

    //
    void ANM_Update_ANIMATION__START()
    {
        Basic_animator.SetTrigger("isOpen");

        //
        Basic_phase = PHASE.AMIMATION__END_CHECK;
    }

    void ANM_Update_AMIMATION__END_CHECK()
    {
        if( (Basic_animator.GetCurrentAnimatorStateInfo(0).IsName(Basic_strs[0])) &&
            (Basic_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f))
        {
            //
            for (int i = 0; i < Basic_objs.Count; i++)
            {
                Grabbable objGrabble = Basic_objs[i].GetComponent<Grabbable>();
                if (objGrabble != null)
                {
                    objGrabble.enabled = true;
                }
            }

            //
            Basic_phase = PHASE.AMIMATION__END;
        }
    }
}
