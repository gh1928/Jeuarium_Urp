using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using BNG;

public partial class ANM_ColorPainting_Obj : GrabbableEvents
{
    [System.Serializable]
    public class TrasnformData
    {
        [System.Serializable]
        public class Datas
        {
            [SerializeField] GameObject Basic_obj;
            [SerializeField] float      Basic_time;

            ////////// Getter & Setter  //////////
            public GameObject   ANM_Basic_obj   { get { return Basic_obj;   }   }
            
            public float        ANM_Basic_time  { get { return Basic_time;  }   }

            ////////// Method           //////////

            ////////// Unity            //////////
        }
        [SerializeField] List<Datas> Basic_steps;

        [Header("RUNNING")]
        [SerializeField] int Basic_phase;

        ////////// Getter & Setter  //////////
        public List<Datas> ANM_Basic_steps  { get { return Basic_steps;                 }   }

        public Datas        ANM_Basic_data  { get { return Basic_steps[Basic_phase];    }   }

        //
        public int      ANM_Basic_phase { get { return Basic_phase;                 }   }

        ////////// Method           //////////
        public bool ANM_Basic_Next()
        {
            bool res = false;

            //
            Basic_phase++;

            if(Basic_phase >= Basic_steps.Count)
            {
                res = true;
            }

            //
            return res;
        }

        ////////// Unity            //////////
    }

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
        AMIMATION__END
    }

    [SerializeField] ANM_Manager Basic_Manager;
    [SerializeField] List<TrasnformData> Basic_transformDatas;
    [SerializeField] List<string> Basic_strs;

    [SerializeField] Animator Basic_animator;

    [Header("RUNNING")]
    [SerializeField] PHASE Basic_phase;
    [SerializeField] List<Vector3> Basic_vec3s;
    [SerializeField] bool isCatch;
    [SerializeField] int Basic_transformDatasNum;
    [SerializeField] float Basic_transformDatasTimer;

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
        Basic_transformDatasNum = -1;
        Basic_phase = PHASE.NONE;
        isCatch = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (Basic_phase)
        {
            case PHASE.NONE:    { ANM_Update__NONE();   }   break;
            
            //
            case PHASE.TRANSFORM__INIT: { ANM_Update__TRANSFORM__INIT();    }   break;
            case PHASE.TRANSFORM:       { ANM_Update__TRANSFORM();          }   break;
            case PHASE.TRANSFORM__END:  { ANM_Update__TRANSFORM__END();     }   break;

            //
            case PHASE.ANIMATION__START:        { ANM_Update__ANIMATION__START();       }   break;
            case PHASE.AMIMATION__END_CHECK:    { ANM_Update__AMIMATION__END_CHECK();   }   break;
        }
    }

    //
    void ANM_Update__NONE()
    {
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
        Basic_transformDatasNum = 0;
        Basic_phase = PHASE.TRANSFORM__INIT;
    }

    void Update_Letter()
    {
        Basic_Manager.ANM_Event_Trigger(this.gameObject);

        //
        this.GetComponent<LetterController>().ANM_Basic_StopRotateCoroutine();
    }

    //
    //
    void ANM_Update__TRANSFORM__INIT()
    {
        if (Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_obj != null)
        {
            if (Basic_vec3s == null)
            {
                Basic_vec3s = new List<Vector3>();
            }
            while (Basic_vec3s.Count < 4)
            {
                Basic_vec3s.Add(new Vector3());
            }

            // position
            Basic_vec3s[0]
                = new Vector3(
                    this.transform.position.x,
                    this.transform.position.y,
                    this.transform.position.z);
            Basic_vec3s[1]
                = new Vector3(
                    Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_obj.transform.position.x - Basic_vec3s[0].x,
                    Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_obj.transform.position.y - Basic_vec3s[0].y,
                    Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_obj.transform.position.z - Basic_vec3s[0].z);

            // rotation
            Basic_vec3s[2]
                = new Vector3(
                    this.transform.rotation.eulerAngles.x,
                    this.transform.rotation.eulerAngles.y,
                    this.transform.rotation.eulerAngles.z);
            Basic_vec3s[3]
                = new Vector3(
                    ANM_Update__TRANSFORM__INIT_CalcRot(Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_obj.transform.rotation.eulerAngles.x - Basic_vec3s[2].x),
                    ANM_Update__TRANSFORM__INIT_CalcRot(Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_obj.transform.rotation.eulerAngles.y - Basic_vec3s[2].y),
                    ANM_Update__TRANSFORM__INIT_CalcRot(Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_obj.transform.rotation.eulerAngles.z - Basic_vec3s[2].z));

            //
            this.transform.parent = Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_obj.transform;
        }

        Basic_transformDatasTimer = 0.0f;

        //
        Basic_phase = PHASE.TRANSFORM;
    }

    float ANM_Update__TRANSFORM__INIT_CalcRot(float _value)
    {
        float res = _value;

        //
        if(_value > 180.0f)
        {
            res -= 360.0f;
        }
        else if(_value < -180.0f)
        {
            res += 360.0f;
        }

        Debug.Log(res);
        //
        return res;
    }

    //
    void ANM_Update__TRANSFORM()
    {
        Basic_transformDatasTimer += Time.deltaTime;
        if (Basic_transformDatasTimer >= Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_time)
        {
            Basic_transformDatasTimer = Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_time;
            //
            Basic_phase = PHASE.TRANSFORM__END;
        }

        if (Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_obj != null)
        {
            float lerp = Basic_transformDatasTimer / Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_time;
            this.transform.position
                = new Vector3(
                    Basic_vec3s[0].x + (Basic_vec3s[1].x * lerp),
                    Basic_vec3s[0].y + (Basic_vec3s[1].y * lerp),
                    Basic_vec3s[0].z + (Basic_vec3s[1].z * lerp));
            this.transform.rotation
                = Quaternion.Euler(
                    Basic_vec3s[2].x + (Basic_vec3s[3].x * lerp),
                    Basic_vec3s[2].y + (Basic_vec3s[3].y * lerp),
                    Basic_vec3s[2].z + (Basic_vec3s[3].z * lerp));

            //this.transform.position
            //    = Vector3.Lerp(
            //        Basic_vec3s[0],
            //        Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_obj.transform.position,
            //        Basic_transformDatasTimer / Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_time);
            //this.transform.rotation
            //    = Quaternion.Euler(
            //        Vector3.Lerp(
            //            Basic_vec3s[1],
            //            Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_obj.transform.rotation.eulerAngles,
            //            Basic_transformDatasTimer / Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_data.ANM_Basic_time));
        }
    }

    void ANM_Update__TRANSFORM__END()
    {
        if (Basic_Manager.ANM_Event_Trigger(this.gameObject))
        {
            if(Basic_transformDatas[Basic_transformDatasNum].ANM_Basic_Next())
            {
                Basic_phase = PHASE.NONE;
            }
            else
            {
                Basic_phase = PHASE.TRANSFORM__INIT;
            }
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
            for (int i = 0; i < Basic_transformDatas[0].ANM_Basic_steps.Count; i++)
            {
                Grabbable objGrabble = Basic_transformDatas[0].ANM_Basic_steps[i].ANM_Basic_obj.GetComponent<Grabbable>();
                if (objGrabble != null)
                {
                    objGrabble.enabled = true;
                }
            }

            //
            Basic_phase = PHASE.AMIMATION__END;
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
        Basic_transformDatasNum = 1;
        Basic_phase = PHASE.TRANSFORM__INIT;
    }
}