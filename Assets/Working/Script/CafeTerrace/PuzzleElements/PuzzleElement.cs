using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Elements
{
    None = -1,
    Lantern = 0,
    Aqua = 1,
}
public abstract class PuzzleElement : MonoBehaviour
{
    public Image mainImage;
    
    private bool placeAtNode = false;

    private bool isWorked = false;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (placeAtNode)
            return;

        if (other.CompareTag("CaffePuzzle"))
            OnWorked();
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (placeAtNode)
            return;

        if (other.CompareTag("CaffePuzzle"))
            OffWorked();
    }

    public void OnPlaceAtNode(PuzzleNode node)
    {
        placeAtNode = true;
        node.Element = this;
    }
    public virtual void OnWorked()
    {
        isWorked = true;        
    }    
    public virtual void OffWorked() => isWorked = false;    
    public virtual void PlaySuccessEffect() { }
    public virtual void PlayFailEffect() { }
    public virtual bool IsWorked() => isWorked;
}
