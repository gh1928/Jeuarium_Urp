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
        [System.Serializable]
        public class ANM_Part
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
                ACTION__GRAB_RELEASE = 20000,
                ACTION__GRAB_ON,
                ACTION__GRAB_OFF,
                ACTION__DOOR_OPEN,
                ACTION__LETTER_FLY,
                ACTION__WAITTING,

                // UI
                UI__TEXT = 30000,

                // ANIMATION
                ANIMATION__START = 40000,
                ANIMATION__END,
            }

            [SerializeField] TYPE Basic_type;
            [SerializeField] List<GameObject> Basic_objs;
            [SerializeField] List<string> Basic_values;

            ////////// Getter & Setter  //////////
            public TYPE ANM_Basic_type { get { return Basic_type; } }

            public GameObject ANM_Basic_GetObj(int _count) { return Basic_objs[_count]; }

            ////////// Method           //////////
            public void ANM_Basic_SceneStart()
            {
                switch (Basic_type)
                {
                    // UI
                    case TYPE.UI__TEXT: { ANM_Basic_BtnNextText(); } break;
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
                for (int i = 0; i < Basic_objs.Count; i++)
                {
                    if (Basic_objs[i].Equals(_obj))
                    {
                        Basic_values[i] = true.ToString();
                    }
                }
            }

            public bool ANM_Basic_TriggerCheck()
            {
                bool res = true;

                for (int i = 0; i < Basic_values.Count; i++)
                {
                    if (!bool.Parse(Basic_values[i]))
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

                    if ((_hands[i].grabber.RemoteGrabbingGrabbable != null) && (_hands[i].grabber.RemoteGrabbingGrabbable.Equals(Basic_objs[0].GetComponent<BNG.Grabbable>())))
                    {
                        _hands[i].grabber.resetFlyingGrabbable();
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

            public void ANM_Basic_LetterFly()
            {
                Basic_objs[0].GetComponent<LetterEvtHandler>().ANM_Basic_LetterFly();
            }

            public bool ANM_Basic_Waitting()
            {
                bool res = false;

                //
                float time = float.Parse(Basic_values[0]) + Time.deltaTime;

                if(time >= float.Parse(Basic_values[1]))
                {
                    res = true;
                }
                else
                {
                    Basic_values[0] = time.ToString();
                }

                //
                return res;
            }

            // UI
            public bool ANM_Basic_BtnNextText()
            {
                bool res = false;

                //
                int num = int.Parse(Basic_values[0]) + 1;

                if (num < Basic_values.Count)
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
            public void ANM_Basic_AnimationStart()
            {
                for (int i = 0; i < Basic_values.Count; i++)
                {
                    string[] strs = Basic_values[i].Split('/');
                    switch (strs[0])
                    {
                        case "Trigger": { Basic_objs[0].GetComponent<Animator>().SetTrigger(strs[1]                     );  }   break;
                        case "Int":     { Basic_objs[0].GetComponent<Animator>().SetInteger(strs[1], int.Parse(strs[2]) );  }   break;
                    }
                }
            }

            public bool ANM_Basic_AnimationEnd()
            {
                bool res = false;

                //
                if (Basic_objs[0].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    res = true;
                }

                //
                return res;
            }

            ////////// Unity            //////////
        }
        [SerializeField] List<ANM_Part> Basic_parts;

        [Header("RUNNING")]
        [SerializeField] int Basic_num;

        ////////// Getter & Setter  //////////
        public ANM_Part ANM_Basic_GetPart(int _count) { return Basic_parts[_count]; }

        ////////// Method           //////////
        public bool ANM_Basic_Trigger(GameObject _obj)
        {
            bool res = false;

            //
            if (Basic_parts[Basic_num].ANM_Basic_type.Equals(ANM_Event.ANM_Part.TYPE.TRIGGER))
            {
                Basic_parts[Basic_num].ANM_Basic_TriggerEnter(_obj);
                res = true;
            }

            return res;
        }

        public void ANM_Basic_BtnNextText()
        {
            if (Basic_parts[Basic_num].ANM_Basic_BtnNextText())
            {
                Basic_num++;

                if (Basic_num < Basic_parts.Count)
                {
                    Basic_parts[Basic_num].ANM_Basic_SceneStart();
                }
            }
        }

        ////////// Unity            //////////
        public void ANM_Basic_Start()
        {
            Basic_num = 0;
        }

        public bool ANM_Basic_Update(List<BNG.HandController> _hands)
        {
            bool res = false;

            if (Basic_num < Basic_parts.Count)
            {
                bool isNext = false;

                switch (Basic_parts[Basic_num].ANM_Basic_type)
                {
                    // OBJECT
                    case ANM_Part.TYPE.OBJECT__ABLE:        { Basic_parts[Basic_num].ANM_Basic_SetActive(true);     isNext = true;  }   break;
                    case ANM_Part.TYPE.OBJECT__DISABLE:     { Basic_parts[Basic_num].ANM_Basic_SetActive(false);    isNext = true;  }   break;
                    case ANM_Part.TYPE.OBJECT__POSITION:    { Basic_parts[Basic_num].ANM_Basic_Position();          isNext = true;  }   break;

                    // TRIGGER
                    case ANM_Part.TYPE.TRIGGER: { isNext = Basic_parts[Basic_num].ANM_Basic_TriggerCheck(); }   break;

                    // ACTION
                    case ANM_Part.TYPE.ACTION__GRAB_RELEASE:    { isNext = Basic_parts[Basic_num].ANM_Basic_GrabRelease(_hands);    }   break;
                    case ANM_Part.TYPE.ACTION__GRAB_ON:         { Basic_parts[Basic_num].ANM_Basic_GrabbableOn();   isNext = true;  }   break;
                    case ANM_Part.TYPE.ACTION__GRAB_OFF:        { Basic_parts[Basic_num].ANM_Basic_GrabbableOff();  isNext = true;  }   break;
                    case ANM_Part.TYPE.ACTION__DOOR_OPEN:       { Basic_parts[Basic_num].ANM_Basic_DoorOpen();      isNext = true;  }   break;
                    case ANM_Part.TYPE.ACTION__LETTER_FLY:      { Basic_parts[Basic_num].ANM_Basic_LetterFly();     isNext = true;  }   break;
                    case ANM_Part.TYPE.ACTION__WAITTING:        { isNext = Basic_parts[Basic_num].ANM_Basic_Waitting();             }   break;

                    // UI

                    // ANIMATION
                    case ANM_Part.TYPE.ANIMATION__START:    { Basic_parts[Basic_num].ANM_Basic_AnimationStart();    isNext = true;  }   break;
                    case ANM_Part.TYPE.ANIMATION__END:      { isNext = Basic_parts[Basic_num].ANM_Basic_AnimationEnd();             }   break;
                }

                //
                if (isNext)
                {
                    Basic_num++;

                    if (Basic_num < Basic_parts.Count)
                    {
                        Basic_parts[Basic_num].ANM_Basic_SceneStart();
                    }
                    else
                    {
                        res = true;
                    }
                }
            }
            else
            {
                res = true;
            }

            return res;
        }
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
        res = Event_list[Event_num].ANM_Basic_Trigger(_obj);

        return res;
    }

    public void ANM_Event_BtnNextText()
    {
        Event_list[Event_num].ANM_Basic_BtnNextText();
    }

    ////////// Unity            //////////
    void ANM_Event_Update()
    {
        if(Event_num < Event_list.Count)
        {
            bool isNext = Event_list[Event_num].ANM_Basic_Update(Hand_hands);

            //
            if (isNext)
            {
                Event_num++;

                if(Event_num < Event_list.Count)
                {
                    Event_list[Event_num].ANM_Basic_Start();
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
    public bool ANM_Hand_GrabRelease(GameObject _obj)
    {
        bool res = false;

        //
        for (int i = 0; i < Hand_hands.Count; i++)
        {
            if (Hand_hands[i].GripAmount > 0)
            {
                if ((Hand_hands[i].PreviousHeldObject != null) && (Hand_hands[i].PreviousHeldObject.Equals(_obj)))
                {
                    Hand_hands[i].grabber.TryRelease();
                    res = true;
                }

                if ((Hand_hands[i].grabber.RemoteGrabbingGrabbable != null) && (Hand_hands[i].grabber.RemoteGrabbingGrabbable.Equals(_obj.GetComponent<BNG.Grabbable>())))
                {
                    Hand_hands[i].grabber.resetFlyingGrabbable();
                    res = true;
                }
            }
        }

        //
        return res;
    }

    ////////// Unity            //////////
}