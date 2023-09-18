using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimParameters : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetActingInteger(int num)
    {
        anim.SetInteger("Acting", num);
    }

    public void setAnnoyingBoolean(bool setBool=true)
    {
        anim.SetBool("Annoying", setBool);
    }

}
