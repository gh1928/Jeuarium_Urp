using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �̺�Ʈ�� �۵���Ű�� Ʈ���ſ� �ش��ϴ� �̺�Ʈ�� �����ϴ� Ŭ�����Դϴ�.
/// Ʈ���� ���� ��ũ��Ʈ�� ������Ʈ�� ������ ������Ʈ�� ���� ����ϰ�,
/// �ش� ��ũ��Ʈ���� �ۼ��� SetTrigger(TriggerSet t)�� ������ ��
/// events ����Ʈ�� Ʈ���Ű� ���� �۵��Ǹ� �߻��� �̺�Ʈ�� �����մϴ�.
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

    // ���� �� TriggerSet ����Ʈ�� ��ϵ� �� ��ŭ �ڷ�ƾ�� �۵����� �� �̺�Ʈ���� Ʈ���Ű� ���� �۵��Ǿ����� Ȯ���մϴ�.
    void Start()
    {
        foreach (TriggerSet t in triggerSet)
        {
            StartCoroutine(t.CheckTriggers());
        }
    }
}
