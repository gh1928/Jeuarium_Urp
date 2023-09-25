using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ANM_Manager : MonoBehaviour
{
    ////////// Getter & Setter  //////////

    ////////// Method           //////////

    ////////// Unity            //////////
    // Start is called before the first frame update
    void Start()
    {
        ANM_UI_Start();
    }

    // Update is called once per frame
    void Update()
    {
        ANM_UI_Update();
        ANM_Event_Update();
    }
}

partial class ANM_Manager
{
    [Header("UI ==================================================")]
    [SerializeField] List<Canvas> UI_canvases;

    [Header("RUNNING")]
    [SerializeField] bool UI_CanvasIsInit;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////

    ////////// Unity            //////////
    void ANM_UI_Start()
    {
        UI_CanvasIsInit = true;
    }

    void ANM_UI_Update()
    {
        if (UI_CanvasIsInit)
        {
            UI_CanvasIsInit = false;

            for (int i = 0; i < UI_canvases.Count; i++)
            {
                if(UI_canvases[i].worldCamera == null)
                {
                    UI_CanvasIsInit = true;
                }
                else
                {
                    UI_canvases[i].gameObject.SetActive(false);
                    UI_canvases[i].transform.localScale = Vector3.one;
                }
            }
        }
    }
}

partial class ANM_Manager
{
    [System.Serializable]
    public class ANM_Event
    {
        public enum TYPE
        {
            // OBJECT
            OBJECT__ABLE = 0,
            OBJECT__DISABLE,
            OBJECT__POSITION,

            // TRIGGER
            TRIGGER = 10000,

            // ACTION
            ACTION__GRAB_RELEASE    = 20000,
            ACTION__GRAB_ON,
            ACTION__GRAB_OFF,
            ACTION__DOOR_OPEN,

            // UI
            UI__TEXT    = 30000,

            // ANIMATION
            ANIMATION__NEXT = 40000,
            ANIMATION__END,
        }

        [SerializeField] TYPE               Basic_type;
        [SerializeField] List<GameObject>   Basic_objs;
        [SerializeField] List<string>       Basic_values;

        ////////// Getter & Setter  //////////
        public TYPE ANM_Basic_type  { get { return Basic_type;  }   }

        public GameObject ANM_Basic_GetObj(int _count)  { return Basic_objs[_count];    }

        ////////// Method           //////////
        public void ANM_Basic_SceneStart()
        {
            switch(Basic_type)
            {
                // UI
                case TYPE.UI__TEXT: { ANM_Basic_BtnNextText();  }   break;
            }
        }

        // OBJECT
        public void ANM_Basic_SetActive(bool _isActive)
        {
            for (int i = 0; i < Basic_objs.Count; i++)
            {
                Basic_objs[i].SetActive(_isActive);
            }
        }

        public void ANM_Basic_Position()
        {
            Basic_objs[0].transform.position = Basic_objs[1].transform.position;
        }

        // TRIGGER
        public void ANM_Basic_TriggerEnter(GameObject _obj)
        {
            for(int i = 0; i < Basic_objs.Count; i++)
            {
                if(Basic_objs[i].Equals(_obj))
                {
                    Basic_values[i] = true.ToString();
                }
            }
        }

        public bool ANM_Basic_TriggerCheck()
        {
            bool res = true;

            for(int i = 0; i < Basic_values.Count; i++)
            {
                if(!bool.Parse(Basic_values[i]))
                {
                    res = false;
                    break;
                }
            }

            return res;
        }

        // ACTION
        public bool ANM_Basic_GrabRelease(List<BNG.HandController> _hands)
        {
            bool res = false;

            //
            for (int i = 0; i < _hands.Count; i++)
            {
                if ((_hands[i].PreviousHeldObject != null) && (_hands[i].PreviousHeldObject.Equals(Basic_objs[0])))
                {
                    _hands[i].grabber.TryRelease();
                    res = true;
                }
            }

            //
            return res;
        }

        public void ANM_Basic_GrabbableOn()
        {
            for (int i = 0; i < Basic_objs.Count; i++)
            {
                Basic_objs[i].GetComponent<BNG.Grabbable>().enabled = true;
            }
        }

        public void ANM_Basic_GrabbableOff()
        {
            for (int i = 0; i < Basic_objs.Count; i++)
            {
                Basic_objs[i].GetComponent<BNG.Grabbable>().enabled = false;
            }
        }

        public void ANM_Basic_DoorOpen()
        {
            Basic_objs[0].GetComponent<DoorController>().TryMoveDoor(float.Parse(Basic_values[0]), float.Parse(Basic_values[1]), true);
        }

        // UI
        public bool ANM_Basic_BtnNextText()
        {
            bool res = false;

            //
            int num = int.Parse(Basic_values[0]) + 1;

            if(num < Basic_values.Count)
            {
                TMPro.TextMeshProUGUI tmPro = Basic_objs[0].GetComponent<TMPro.TextMeshProUGUI>();
                tmPro.text = Basic_values[num].Replace("\\n", "\n");

                Basic_values[0] = num.ToString();
            }
            else
            {
                res = true;
            }

            //
            return res;
        }

        // ANIMATION
        public void ANM_Basic_AnimationNext()
        {
            Basic_objs[0].GetComponent<Animator>().SetTrigger("Next");
        }

        public bool ANM_Basic_AnimationEnd()
        {
            bool res = false;

            //
            if(Basic_objs[0].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                res = true;
            }

            //
            return res;
        }

        ////////// Unity            //////////
    }

    [Header("EVENT ==================================================")]
    [SerializeField] List<ANM_Event> Event_list;

    [Header("RUNNING")]
    [SerializeField] int Event_num; // 현재 실행중인 이벤트 번호

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public bool ANM_Event_Trigger(GameObject _obj)
    {
        bool res = false;

        //
        if(Event_list[Event_num].ANM_Basic_type.Equals(ANM_Event.TYPE.TRIGGER))
        {
            Event_list[Event_num].ANM_Basic_TriggerEnter(_obj);
            res = true;
        }

        return res;
    }

    public void ANM_Event_BtnNextText()
    {
        if(Event_list[Event_num].ANM_Basic_BtnNextText())
        {
            Event_num++;

            if (Event_num < Event_list.Count)
            {
                Event_list[Event_num].ANM_Basic_SceneStart();
            }
        }
    }

    ////////// Unity            //////////
    void ANM_Event_Update()
    {
        if(Event_num < Event_list.Count)
        {
            bool isNext = false;

            switch (Event_list[Event_num].ANM_Basic_type)
            {
                // OBJECT
                case ANM_Event.TYPE.OBJECT__ABLE:       { Event_list[Event_num].ANM_Basic_SetActive(true);  isNext = true;  }   break;
                case ANM_Event.TYPE.OBJECT__DISABLE:    { Event_list[Event_num].ANM_Basic_SetActive(false); isNext = true;  }   break;
                case ANM_Event.TYPE.OBJECT__POSITION:   { Event_list[Event_num].ANM_Basic_Position();       isNext = true;  }   break;

                // TRIGGER
                case ANM_Event.TYPE.TRIGGER:    { isNext = Event_list[Event_num].ANM_Basic_TriggerCheck();  }   break;

                // ACTION
                case ANM_Event.TYPE.ACTION__GRAB_RELEASE:   { isNext = Event_list[Event_num].ANM_Basic_GrabRelease(Hand_hands); }   break;
                case ANM_Event.TYPE.ACTION__GRAB_ON:        { Event_list[Event_num].ANM_Basic_GrabbableOn();    isNext = true;  }   break;
                case ANM_Event.TYPE.ACTION__GRAB_OFF:       { Event_list[Event_num].ANM_Basic_GrabbableOff();   isNext = true;  }   break;
                case ANM_Event.TYPE.ACTION__DOOR_OPEN:      { Event_list[Event_num].ANM_Basic_DoorOpen();       isNext = true;  }   break;

                // UI

                // ANIMATION
                case ANM_Event.TYPE.ANIMATION__NEXT:    { Event_list[Event_num].ANM_Basic_AnimationNext();  isNext = true;  }   break;
                case ANM_Event.TYPE.ANIMATION__END:     { isNext = Event_list[Event_num].ANM_Basic_AnimationEnd();          }   break;
            }

            //
            if (isNext)
            {
                Event_num++;

                if(Event_num < Event_list.Count)
                {
                    Event_list[Event_num].ANM_Basic_SceneStart();
                }
            }
        }
    }
}

partial class ANM_Manager
{
    [Header("HAND_CONTROLLER ==================================================")]
    [SerializeField] List<BNG.HandController> Hand_hands;

    ////////// Getter & Setter  //////////

    ////////// Method           //////////
    public void ANM_Hand_GrabRelease(GameObject _obj)
    {
        for (int i = 0; i < Hand_hands.Count; i++)
        {
            if ((Hand_hands[i].PreviousHeldObject != null) && (Hand_hands[i].PreviousHeldObject.Equals(_obj)))
            {
                Hand_hands[i].grabber.TryRelease();
            }
        }
    }

    ////////// Unity            //////////
}