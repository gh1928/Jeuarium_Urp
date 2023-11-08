using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using BNG;

public partial class ANM_GoghYellowhouse_Letter : GrabbableEvents
{
    enum PHASE
    {
        INIT,

        //
        NONE,

        //
        OPEN_START,
        OPEN,
        OPEN_END,

        //
        MOVE_START,
        MOVE,
        MOVE_END
    }
    [SerializeField] ANM_GoghYellowhouse_Manager Basic_manager;
    [SerializeField] List<Grabbable> Basic_Grabbables;

    [Header("RUNNING")]
    [SerializeField] bool Basic_isCatch;
    [SerializeField] PHASE Basic_phase;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    // OnGrab
    //public override void OnGrab(BNG.Grabber _grabber)
    //{
    //    base.OnGrab(_grabber);
    //
    //    Basic_isCatch = true;
    //}

    public override void OnBecomesClosestRemoteGrabbable(Grabber _theGrabber)
    {
        base.OnBecomesClosestRemoteGrabbable(_theGrabber);

        Basic_isCatch = true;
    }

    public override void OnNoLongerClosestRemoteGrabbable(Grabber _theGrabber)
    {
        base.OnNoLongerClosestRemoteGrabbable(_theGrabber);

        Basic_isCatch = false;
    }

    public void ANM_Basic_BtnOK()
    {
        this.gameObject.SetActive(false);
        for(int i = 0; i < Basic_Grabbables.Count; i++)
        {
            Basic_Grabbables[i].enabled = true;
        }
    }

    ////////// Unity            //////////
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Basic_isCatch)
        {
            bool isClick = false;

            if (InputBridge.Instance.RightGripDown)
            {
                isClick = Basic_manager.ANM_Hand_GrabRelease(ANM_GoghYellowhouse_Manager.Hand_Type.RIGHT, this.gameObject);
            }

            if (!isClick)
            {
                if (InputBridge.Instance.LeftGripDown)
                {
                    isClick = Basic_manager.ANM_Hand_GrabRelease(ANM_GoghYellowhouse_Manager.Hand_Type.LEFT, this.gameObject);
                }
            }

            //
            if (isClick)
            {
                Basic_phase = PHASE.OPEN_START;
            }
        }

        switch(Basic_phase)
        {
            case PHASE.INIT:
                {
                    if (ANM_Open_Init())
                    {
                        Basic_phase = PHASE.NONE;
                    }
                }
                break;

            //
            case PHASE.NONE:    {   }   break;
            
            //
            case PHASE.OPEN_START:  {       ANM_Open_Update__OPEN_START();      Basic_phase = PHASE.OPEN;           }   break;
            case PHASE.OPEN:        { if (  ANM_Open_Update__OPEN()         ) { Basic_phase = PHASE.OPEN_END;   }   }   break;
            case PHASE.OPEN_END:    {       ANM_Open_Update__OPEN_END();        Basic_phase = PHASE.NONE;           }   break;
        }
    }
}

partial class ANM_GoghYellowhouse_Letter
{
    [Header("OPEN ==================================================")]
    [SerializeField] Animator   Open_animator;
    [SerializeField] Canvas     Open_canvas;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////

    ////////// Unity            //////////
    //
    bool ANM_Open_Init()
    {
        bool res = false;

        if (Open_canvas.worldCamera != null)
        {
            Open_canvas.gameObject.SetActive(false);
            res = true;
        }
        else
        {
        }

        return res;
    }

    //
    void ANM_Open_Update__OPEN_START()
    {
        Open_animator.SetTrigger("Next");
        this.GetComponent<UnityEngine.Animations.LookAtConstraint>().constraintActive = true;
        this.GetComponent<BNG.Grabbable>().enabled = false;
    }

    bool ANM_Open_Update__OPEN()
    {
        bool res = false;

        //
        if( (Open_animator.GetCurrentAnimatorStateInfo(0).IsName("Open")) &&
            (Open_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f))
        {
            Open_animator.gameObject.SetActive(false);
            Open_canvas.gameObject.SetActive(true);

            res = true;
        }

        //
        return res;
    }

    void ANM_Open_Update__OPEN_END()
    {

    }
}