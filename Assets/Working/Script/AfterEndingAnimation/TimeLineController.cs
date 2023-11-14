using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineController : MonoBehaviour
{
    private PlayableDirector playableDirector;

    private void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }
}
