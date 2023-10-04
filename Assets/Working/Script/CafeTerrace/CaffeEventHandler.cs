using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaffeEvent;

namespace CaffeEvent
{
    public enum CaffeEvents
    {
        None = -1,
        Window = 0,
    }
}
public class CaffeEventHandler : MonoBehaviour
{   
    public GameObject[] events;
}
