using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 이벤트를 작동시키는 트리거와 해당하는 이벤트를 관리하는 클래스입니다.
/// 트리거 관련 스크립트가 컴포넌트로 부착된 오브젝트를 먼저 등록하고,
/// 해당 스크립트에서 작성된 SetTrigger(TriggerSet t)를 지정한 뒤
/// events 리스트에 트리거가 전부 작동되면 발생할 이벤트를 지정합니다.
/// </summary>
[Serializable]
public class TriggerSet
{
    public UnityEvent<TriggerSet> triggers;
    public UnityEvent events;
    List<bool> isTriggered;
    int i = 0;

    TriggerSet()
    {
        isTriggered = new List<bool>();
    }

    public void AddTrigger()
    {
        isTriggered.Add(false);
    }

    public void SwitchOn()
    {
        isTriggered[i] = true;
        i++;
    }

    public IEnumerator CheckTriggers()
    {
        triggers.Invoke(this);

        yield return new WaitUntil(() =>
        {
            foreach (var trigger in isTriggered)
            {
                if (!trigger) return false;
            }                
            return true;
        });

        events.Invoke();
    }
}

public class RoomTriggerManager : MonoBehaviour
{
    public List<TriggerSet> triggerSet;

    // 시작 시 TriggerSet 리스트에 등록된 수 만큼 코루틴을 작동시켜 각 이벤트마다 트리거가 전부 작동되었는지 확인합니다.
    void Start()
    {
        foreach (TriggerSet t in triggerSet)
        {
            StartCoroutine(t.CheckTriggers());
        }
    }
}
