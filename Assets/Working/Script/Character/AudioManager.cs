using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public struct Dialog
    {
        public AudioClip DialogClip;
        public string[] DialogText;
        public float[] DialogTime;
    }

    Animator Anim;
    AudioSource AudioSource;

    [SerializeField] Dialog[] dialogs;
    public AudioClip[] WalkAudioClips;

    public GameObject DialogUI;
    public Text DialogUIText;

    bool PlayDialog=false;

    private void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        if (GetComponent<Animator>())
            Anim = GetComponent<Animator>();
        StartCoroutine(MouseMove());
    }

    public void WalkSound(int clipNum) {
        AudioSource.clip = WalkAudioClips[clipNum];
        AudioSource.Play();
    }

    public void DialogSound(int clipNum)
    {
        StartCoroutine(DialogTextSetting(clipNum));
    }

    public void StopDialogSound()
    {
        AudioSource.Stop();
    }

    IEnumerator DialogTextSetting(int clipNum)
    {

        DialogUI.SetActive(true);
        PlayDialog = true;

        AudioSource.clip = dialogs[clipNum].DialogClip;
        AudioSource.Play();
        Anim.SetBool("Talking",true);
        int count=0;
        while (AudioSource.isPlaying)
        {
            if (count == dialogs[clipNum].DialogText.Length)
            {
                AudioSource.Stop();

                Debug.Log("break");
                break;
            }  

            if(Anim.GetInteger("Acting")==0&&Anim.GetFloat("Forward")==0)
                Anim.SetInteger("Acting", Random.Range(1, 4));          
            
            DialogUIText.text = dialogs[clipNum].DialogText[count];
            yield return new WaitForSeconds(dialogs[clipNum].DialogTime[count++]);
        }
        if (Anim.GetInteger("Acting") > 0&& Anim.GetInteger("Acting") <5)
            Anim.SetInteger("Acting", 0);
        Anim.SetBool("Talking", false);
        DialogUI.SetActive(false);
        PlayDialog = false;
    }

    IEnumerator MouseMove()
    {
        while (true)
        {
            Anim.SetTrigger("Blink");
            yield return new WaitForSeconds(Random.Range(5,6));
        }
    }

    private void LateUpdate()
    {
        if (PlayDialog)
        {
            DialogUI.transform.LookAt(Camera.main.transform);
            DialogUI.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }
}
