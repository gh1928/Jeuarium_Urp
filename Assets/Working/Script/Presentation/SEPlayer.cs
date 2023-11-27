using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEPlayer : MonoBehaviour
{
    public float deley = 5f;

    private float lastClipPlaytime;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lastClipPlaytime = - deley;
    }
    // Update is called once per frame
    void Update()
    {
        if ((Time.time - lastClipPlaytime) < deley)
            return;

        lastClipPlaytime = Time.time;

        audioSource.Play();
    }
}
