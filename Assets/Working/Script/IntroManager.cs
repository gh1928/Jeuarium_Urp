using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[Serializable]
public class IntroEvent
{
    public int index = 0;
    public UnityEvent events;
}


public class IntroManager : MonoBehaviour
{
    public CharacterController cc;
    public TextMeshProUGUI tm;

    public int endPoint = 0;

    [TextArea(minLines:4, maxLines:6)]
    public List<string> text;

    public List<IntroEvent> introEvents;

    int i = 0;
    Vector3 pos;
    Quaternion rot;

    void Start()
    {
        pos = cc.transform.position;
        rot = cc.transform.rotation;

        transform.position = cc.transform.position + cc.transform.up * 0.5f + cc.transform.forward * 2f;
        NextText();
    }

    void NextText()
    {
        if (i == endPoint)
        {
            cc.enabled = true;
            endPoint++;
            gameObject.SetActive(false);
            return;
        }

        tm.text = text[i];
        i++;
    }

    public void SkipText()
    {
        i++;
        endPoint++;
        ResetIntro();
    }

    public void ResetIntro()
    {
        cc.enabled = false;
        gameObject.SetActive(true);
        transform.position = cc.transform.position + cc.transform.up * 0.5f + cc.transform.forward * 2f;
        NextText();
    }

    public void EventCheck()
    {
        foreach (IntroEvent e in introEvents)
        {
            if (e.index == i)
            {
                e.events.Invoke();
            }
        }

        NextText();
    }

    public void ResetPlayerTransform()
    {
        cc.transform.SetPositionAndRotation(pos, rot);
        transform.position = cc.transform.position + cc.transform.up * 0.5f + cc.transform.forward * 2f;
    }
}
