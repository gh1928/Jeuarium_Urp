using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PuzzleElement : MonoBehaviour
{
    public Image mainImage;

    [Header("위치는 간선이나 노드에 존재")]
    public bool atNode = false;

    private bool isWorked = false;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (atNode)
            return;

        if(other.CompareTag("CaffePuzzle"))
            OnWorked();
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (atNode)
            return;

        if (other.CompareTag("CaffePuzzle"))
            OffWorked();
    }

    protected virtual void OnWorked() => isWorked = true;
    protected virtual void OffWorked() => isWorked = false;    
    public virtual void WorkedEffect() { }
    public virtual void FailedEffect() { }
    public virtual bool CheckWorked()
    {
        if(isWorked)
            WorkedEffect();

        if(!isWorked)
            FailedEffect();

        return isWorked;
    }
}
