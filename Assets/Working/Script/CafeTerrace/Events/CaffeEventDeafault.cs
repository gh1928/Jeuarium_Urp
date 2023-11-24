using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CaffeEventDeafault : MonoBehaviour
{
    public float totalEvtTime = 15f;
    public virtual void OnEvent(List<PuzzleElement> elements) { }
    public float GetEvtTime() => totalEvtTime;
}
