using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PuzzleElement : MonoBehaviour
{
    public Image mainImage;

    private bool isWorked = false;

    protected virtual void OnTriggerEnter(Collider other) => OnWorked();
    protected virtual void OnTriggerExit(Collider other) => OffWorked();

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
