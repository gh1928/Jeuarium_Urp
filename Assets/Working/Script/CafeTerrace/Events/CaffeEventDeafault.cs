using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CaffeEventDeafault : MonoBehaviour
{
    public virtual void OnEvent(List<PuzzleElement> elements) { }
}
