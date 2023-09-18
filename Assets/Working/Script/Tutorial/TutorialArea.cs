using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArea : MonoBehaviour
{
    public TutorialManager tutorialManager;
    public GameObject Glow,Tool;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Glow.SetActive(false);
            transform.GetComponent<CapsuleCollider>().enabled = false;
            Tool.SetActive(true);
        }
    }

    public bool isCanvasGrab=false;
    public bool isCanvasRelease= false;
    public bool isCrayonGrab = false;
    public void GrabCanvasEvent()
    {
        if (!isCanvasGrab&&tutorialManager.isGrabTutorial)
        {
            tutorialManager.SetNextTutorial(12);
            isCanvasGrab = true;
        }
    }
    public void ReleaseCanvasEvent()
    {
        if (!isCanvasRelease&& isCanvasGrab)
        {
            tutorialManager.SetNextTutorial(13);
            isCanvasRelease = true;
            tutorialManager.isGrabTutorial = false;
        }
    }
    public void GrabCrayonEvent()
    {
        if (!isCrayonGrab && tutorialManager.isGrabTutorial)
        {
            tutorialManager.SetNextTutorial(18);
            isCrayonGrab = true;
        }
    }
}
