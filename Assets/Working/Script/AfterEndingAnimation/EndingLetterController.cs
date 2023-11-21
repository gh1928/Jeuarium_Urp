using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingLetterController : MonoBehaviour
{
    public GameObject letterOnHand;
    public void OnHand() => letterOnHand.SetActive(true);
}
