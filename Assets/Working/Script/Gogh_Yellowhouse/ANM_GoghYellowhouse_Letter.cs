using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using TMPro;
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
    [SerializeField] ANM_Manager Basic_manager;

    [Header("RUNNING")]
    //[SerializeField] bool Basic_isCatch;
    [SerializeField] List<bool> Basic_isCatchs;
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

        //Basic_isCatch = true;
        for(int i = 0; i < Basic_isCatchs.Count; i++)
        {
            if(Basic_manager.ANM_Player_GetHandGrabber((ANM_Manager.Hand_TYPE)i).Equals(_theGrabber))
            {
                Basic_isCatchs[i] = true;
                break;
            }
        }
    }

    public override void OnNoLongerClosestRemoteGrabbable(Grabber _theGrabber)
    {
        base.OnNoLongerClosestRemoteGrabbable(_theGrabber);

        //Basic_isCatch = false;
        for (int i = 0; i < Basic_isCatchs.Count; i++)
        {
            if (Basic_manager.ANM_Player_GetHandGrabber((ANM_Manager.Hand_TYPE)i).Equals(_theGrabber))
            {
                Basic_isCatchs[i] = false;
                break;
            }
        }
    }

    public void ANM_Basic_BtnOK()
    {
        Basic_manager.ANM_Event_Trigger(this.gameObject);
    }

    ////////// Unity            //////////
    // Start is called before the first frame update
    void Start()
    {
        ANM_Move_Start();
    }

    // Update is called once per frame
    void Update()
    {
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
            case PHASE.NONE:    { ANM_Basic_Update__NONE(); }   break;
            
            //
            case PHASE.OPEN_START:  {       ANM_Open_Update__OPEN_START();      Basic_phase = PHASE.OPEN;           }   break;
            case PHASE.OPEN:        { if (  ANM_Open_Update__OPEN()         ) { Basic_phase = PHASE.OPEN_END;   }   }   break;
            case PHASE.OPEN_END:    {       ANM_Open_Update__OPEN_END();        Basic_phase = PHASE.NONE;           }   break;

            //
            case PHASE.MOVE_START:  {       ANM_Move_Update__MOVE_START();      Basic_phase = PHASE.MOVE;           }   break;
            case PHASE.MOVE:        { if(   ANM_Move_Update__MOVE()         ) { Basic_phase = PHASE.MOVE_END;   }   }   break;
            case PHASE.MOVE_END:    {       ANM_Move_Update__MOVE_END();        Basic_phase = PHASE.NONE;           }   break;
        }
    }

    void ANM_Basic_Update__NONE()
    {
        ANM_Basic_Update__NONE__Hand( ANM_Manager.Hand_TYPE.LEFT    );
        ANM_Basic_Update__NONE__Hand( ANM_Manager.Hand_TYPE.RIGHT   );
    }

    void ANM_Basic_Update__NONE__Hand(ANM_Manager.Hand_TYPE _type)
    {
        if (Basic_isCatchs[(int)_type])
        {
            bool isClick = false;

            //
            bool isBtn = false;
            switch(_type)
            {
                case ANM_Manager.Hand_TYPE.LEFT:    { isBtn = (InputBridge.Instance.LeftGrip > 0.95f);  }   break;
                case ANM_Manager.Hand_TYPE.RIGHT:   { isBtn = (InputBridge.Instance.RightGrip > 0.95f); }   break;
            }
            if (isBtn)
            {
                isClick = Basic_manager.ANM_Hand_GrabRelease(_type, this.gameObject);
            }

            //
            if (isClick)
            {
                Basic_isCatchs[(int)_type] = false;
                Basic_manager.ANM_Event_Trigger(this.gameObject);
            }
        }
    }
}

partial class ANM_GoghYellowhouse_Letter
{
    [Header("OPEN ==================================================")]
    [SerializeField] Animator   Open_animator;
    [SerializeField] Canvas     Open_canvas;
    [SerializeField] TMP_Text   Open_text;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public void ANM_Open_Setting()
    {
        Basic_phase = PHASE.OPEN_START;
    }

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
        this.GetComponent<BNG.Grabbable>().enabled = false;
        Open_text.text = Move_datas[Move_num].ANM_Basic_str;
    }

    bool ANM_Open_Update__OPEN()
    {
        bool res = false;

        //
        if( (Open_animator.GetCurrentAnimatorStateInfo(0).IsName("Open")) &&
            (Open_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f))
        {
            res = true;
        }

        //
        return res;
    }

    void ANM_Open_Update__OPEN_END()
    {
        Open_animator.SetTrigger("Next");

        //
        Basic_manager.ANM_Event_Trigger(this.gameObject);
    }
}

partial class ANM_GoghYellowhouse_Letter
{
    [System.Serializable]
    public class Move_Data
    {
        [SerializeField] Transform  Basic_startPoint;
        [SerializeField] float      Basic_time;
        [SerializeField] string     Basic_str;

        ////////// Getter & Setter  //////////
        public Transform ANM_Basic_startPoint { get { return Basic_startPoint; } }

        public float ANM_Basic_time { get { return Basic_time; } }

        public string ANM_Basic_str { get { return Basic_str; } }

        ////////// Method           //////////

        ////////// Unity            //////////
    }

    [Header("MOVE ==================================================")]
    [SerializeField] List<Move_Data> Move_datas;

    [Header("RUNNING")]
    [SerializeField] int Move_num;
    [SerializeField] float Move_timer;
    [SerializeField] float Move_rotateSpeed;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public void ANM_Move_Setting(int _num)
    {
        Open_animator.gameObject.SetActive(false);
        Open_canvas.gameObject.SetActive(false);
        this.gameObject.SetActive(true);

        Basic_phase = PHASE.MOVE_START;

        Move_num = _num;
    }

    ////////// Unity            //////////
    void ANM_Move_Start()
    {
        Move_num = -1;
    }

    //
    void ANM_Move_Update__MOVE_START()
    {
        this.GetComponent<Grabbable>().enabled = false;
        this.GetComponent<UnityEngine.Animations.LookAtConstraint>().constraintActive = false;

        this.transform.rotation = Quaternion.Euler(Vector3.zero);
        Open_animator.transform.position = Move_datas[Move_num].ANM_Basic_startPoint.position;
        Open_animator.gameObject.SetActive(true);

        Move_timer = 0.0f;
        Move_rotateSpeed = 360.0f / Move_datas[Move_num].ANM_Basic_time;
    }

    bool ANM_Move_Update__MOVE()
    {
        bool res = false;

        //
        Move_timer += Time.deltaTime;
        if(Move_timer >= Move_datas[Move_num].ANM_Basic_time)
        {
            Move_timer = Move_datas[Move_num].ANM_Basic_time;
            res = true;
        }

        this.transform.rotation = Quaternion.Euler(0, Move_rotateSpeed * Move_timer, 0);
        Open_animator.transform.position
            = Vector3.Lerp(
                Move_datas[Move_num].ANM_Basic_startPoint.position,
                this.transform.position,
                Move_timer / Move_datas[Move_num].ANM_Basic_time);

        //
        return res;
    }

    void ANM_Move_Update__MOVE_END()
    {
        Basic_manager.ANM_Event_Trigger(this.gameObject);

        this.GetComponent<Grabbable>().enabled = true;
        this.GetComponent<UnityEngine.Animations.LookAtConstraint>().constraintActive = true;
    }
}