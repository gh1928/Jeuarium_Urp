using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryCharacter : MonoBehaviour
{
    public enum Story
    {
        none,
        Monet,
    }

    public Story story;
    public CapsuleCollider capsuleCollider;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene((int)story);
        }
    }
}
