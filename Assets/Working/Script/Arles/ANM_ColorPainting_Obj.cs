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
        TRANSFORM__INIT = 1000,
        TRANSFORM,
        TRANSFORM__END,
        //
        ANIMATION__START = 2000,
        AMIMATION__END_CHECK,
        AMIMATION__END,
        //
        ACTION__PAINT_WAIT_INIT,
        ACTION__PAINT_WAIT,
    }

    [SerializeField] ANM_Manager Basic_Manager;
    [SerializeField] List<GameObject> Basic_objs;
    [SerializeField] List<string> Basic_strs;
    [SerializeField] List<float> Basic_floats;

    [SerializeField] Animator Basic_animator;

    [Header("RUNNING")]
    [SerializeField] PHASE Basic_phase;
    [SerializeField] int Basic_transformDesNum;
    [SerializeField] List<Vector3> Basic_vec3s;
    [SerializeField] bool isCatch;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    // OnGrab
    public override void OnGrab(BNG.Grabber _grabber)
    {
        base.OnGrab(_grabber);

        isCatch = true;
    }

    public override void OnBecomesClosestRemoteGrabbable(Grabber _theGrabber)
    {
        base.OnBecomesClosestRemoteGrabbable(_theGrabber);

        isCatch = true;
    }

    public override void OnNoLongerClosestRemoteGrabbable(Grabber _theGrabber)
    {
        base.OnNoLongerClosestRemoteGrabbable(_theGrabber);

        isCatch = false;
    }

    ////////// Unity            //////////
    // Start is called before the first frame update
    void Start()
    {
        Basic_phase = PHASE.NONE;
        isCatch = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (Basic_phase)
        {
            case PHASE.TRANSFORM__INIT: { ANM_Update__TRANSFORM__INIT();    }   break;
            case PHASE.TRANSFORM:       { ANM_Update__TRANSFORM();          }   break;
            case PHASE.TRANSFORM__END:  { ANM_Update__TRANSFORM__END();     }   break;

            //
            case PHASE.ANIMATION__START:        { ANM_Update__ANIMATION__START();       }   break;
            case PHASE.AMIMATION__END_CHECK:    { ANM_Update__AMIMATION__END_CHECK();   }   break;

            case PHASE.ACTION__PAINT_WAIT_INIT: { ANM_Update__ACTION__PAINT_WAIT_INIT();    }   break;
            case PHASE.ACTION__PAINT_WAIT:      { ANM_Update__ACTION__PAINT_WAIT();         }   break;
        }

        if(isCatch)
        {
            isCatch = !Basic_Manager.ANM_Hand_GrabRelease(this.gameObject);

            if(!isCatch)
            {
                switch (this.tag)
                {
                    case "Ball":    { Update_Ball();    }   break;
                    case "Paint":   { Update_Paint();   }   break;
                    case "Letter":  { Update_Letter();  }   break;
                }
            }
        }
    }

    void Update_Ball()
    {
        GetComponent<Grabbable>().enabled = false;

        //
        Basic_phase = PHASE.ANIMATION__START;
    }

    void Update_Paint()
    {
        GetComponent<Grabbable>().enabled = false;

        //
        Basic_transformDesNum = 0;
        Basic_phase = PHASE.TRANSFORM__INIT;
    }

    void Update_Letter()
    {
        Basic_Manager.ANM_Event_Trigger(this.gameObject);

        //
        this.GetComponent<LetterController>().ANM_Basic_StopRotateCoroutine();
    }

    //
    void ANM_Update__TRANSFORM__INIT()
    {
        if (Basic_vec3s == null)
        {
            Basic_vec3s = new List<Vector3>();
        }
        while(Basic_vec3s.Count < 2)
        {
            Basic_vec3s.Add(new Vector3());
        }
        Basic_vec3s[0] = new Vector3(   this.transform.position.x,              this.transform.position.y,              this.transform.position.z               );
        Basic_vec3s[1] = new Vector3(   this.transform.rotation.eulerAngles.x,  this.transform.rotation.eulerAngles.y,  this.transform.rotation.eulerAngles.z   );

        Basic_floats[0] = 0.0f;

        this.transform.parent = Basic_objs[Basic_transformDesNum].transform;

        //
        Basic_phase = PHASE.TRANSFORM;
    }

    void ANM_Update__TRANSFORM()
    {
        Basic_floats[0] += Time.deltaTime;
        if (Basic_floats[0] >= Basic_floats[1])
        {
            Basic_floats[0] = Basic_floats[1];
            //
            Basic_phase = PHASE.TRANSFORM__END;
        }

        this.transform.position = Vector3.Lerp(                     Basic_vec3s[0], Basic_objs[Basic_transformDesNum].transform.position,               Basic_floats[0] / Basic_floats[1]);
        this.transform.rotation = Quaternion.Euler( Vector3.Lerp(   Basic_vec3s[1], Basic_objs[Basic_transformDesNum].transform.rotation.eulerAngles,   Basic_floats[0] / Basic_floats[1]));
    }

    void ANM_Update__TRANSFORM__END()
    {
        if (Basic_Manager.ANM_Event_Trigger(this.gameObject))
        {
            Basic_phase = PHASE.NONE;
        }
    }

    //
    void ANM_Update__ANIMATION__START()
    {
        for(int i = 1; i < Basic_strs.Count; i++)
        {
            Debug.Log(Basic_strs[i]);
            string[] strs = Basic_strs[i].Split('/');
            switch(strs[0])
            {
                case "Trigger": { Basic_animator.SetTrigger(    strs[1]                         );  }   break;
                case "Int":     { Basic_animator.SetInteger(    strs[1],    int.Parse(strs[2])  );  }   break;
            }
        }

        //
        Basic_phase = PHASE.AMIMATION__END_CHECK;
    }

    void ANM_Update__AMIMATION__END_CHECK()
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

    //
    void ANM_Update__ACTION__PAINT_WAIT_INIT()
    {
        Basic_floats[2] = 0.0f;
        Basic_phase = PHASE.ACTION__PAINT_WAIT;
    }

    void ANM_Update__ACTION__PAINT_WAIT()
    {
        Basic_floats[2] += Time.deltaTime;

        if (Basic_floats[2] >= Basic_floats[3])
        {
            Basic_transformDesNum = 1;
            Basic_phase = PHASE.TRANSFORM__INIT;
        }
    }

    // OnTriggerEnter
    //
    private void OnTriggerEnter(Collider _other)
    {
        switch (_other.tag)
        {
            case "Brush":   { ANM_OnTriggerEnter_Brush();   }   break;
        }
    }

    void ANM_OnTriggerEnter_Brush()
    {
        this.transform.Find("Gray"  ).gameObject.SetActive(false    );
        this.transform.Find("Color" ).gameObject.SetActive(true     );

        //
        Basic_phase = PHASE.ACTION__PAINT_WAIT_INIT;
    }
}
